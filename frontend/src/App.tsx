import { useEffect, useRef, useState } from 'react'
import {
  HubConnectionBuilder,
  type HubConnection,
} from '@microsoft/signalr'
import './App.css'

interface BehaviorOption {
  id: string
  description: string
}

interface Scenario {
  id: string
  name: string
  staffRole: string
  expectedBehavior: string
  behaviorOptions: BehaviorOption[]
}

interface TrainingDebrief {
  simulationId: string
  scenarioName: string
  staffRole: string
  observedBehavior: string
  expectedBehavior: string
  outcome: 'MeetsExpectations' | 'PracticeRecommended'
  aiReflectionQuestion: string
  completedAt: string
}

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5277'

function plainTextAiResponse(text: string) {
  return text
    .replace(/[*_#]/g, '')
    .replace(/\s+/g, ' ')
    .trim()
}

async function getApiError(response: Response) {
  const fallbackMessage = `Request failed (HTTP ${response.status}).`

  try {
    const body = await response.json() as { title?: string; detail?: string }
    return body.detail ?? body.title ?? fallbackMessage
  } catch (parseError) {
    console.error('Could not read the API error response.', parseError)
    return fallbackMessage
  }
}

function App() {
  const [scenarios, setScenarios] = useState<Scenario[]>([])
  const [scenarioId, setScenarioId] = useState('')
  const [behaviorOptionId, setBehaviorOptionId] = useState('')
  const [debrief, setDebrief] = useState<TrainingDebrief | null>(null)
  const [isLoadingScenarios, setIsLoadingScenarios] = useState(true)
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [isWaitingForDebrief, setIsWaitingForDebrief] = useState(false)
  const [error, setError] = useState('')
  const [status, setStatus] = useState('')
  const activeConnection = useRef<HubConnection | null>(null)
  const resultTimeout = useRef<number | null>(null)

  useEffect(() => {
    const controller = new AbortController()

    async function loadScenarios() {
      try {
        const response = await fetch(`${apiBaseUrl}/api/scenarios`, {
          signal: controller.signal,
        })
        if (!response.ok) {
          throw new Error(`Could not load scenarios (HTTP ${response.status}).`)
        }

        const availableScenarios: Scenario[] = await response.json()
        setScenarios(availableScenarios)
        if (availableScenarios.length > 0) {
          setScenarioId(availableScenarios[0].id)
          setBehaviorOptionId(availableScenarios[0].behaviorOptions[0]?.id ?? '')
        }
      } catch (loadError) {
        if (!controller.signal.aborted) {
          setError(loadError instanceof Error ? loadError.message : 'Could not load scenarios.')
        }
      } finally {
        if (!controller.signal.aborted) {
          setIsLoadingScenarios(false)
        }
      }
    }

    void loadScenarios()
    return () => controller.abort()
  }, [])

  useEffect(() => {
    return () => {
      if (resultTimeout.current !== null) {
        window.clearTimeout(resultTimeout.current)
      }

      const connection = activeConnection.current
      if (connection) {
        void connection.stop().catch((stopError: unknown) => {
          console.error('Could not stop the training result connection.', stopError)
        })
      }
    }
  }, [])

  const selectedScenario = scenarios.find((scenario) => scenario.id === scenarioId)

  function selectScenario(nextScenarioId: string) {
    const nextScenario = scenarios.find((scenario) => scenario.id === nextScenarioId)
    setScenarioId(nextScenarioId)
    setBehaviorOptionId(nextScenario?.behaviorOptions[0]?.id ?? '')
    setDebrief(null)
    setError('')
    setStatus('')
  }

  async function startSimulation() {
    if (!selectedScenario || !behaviorOptionId) {
      setError('Choose a scenario and a behavior before starting.')
      return
    }

    const simulationId = crypto.randomUUID()
    const connection = new HubConnectionBuilder()
      .withUrl(`${apiBaseUrl}/hubs/training-results`)
      .withAutomaticReconnect()
      .build()

    let receivedResult = false
    let timedOut = false
    let intentionalStop = false
    const clearResultTimeout = () => {
      if (resultTimeout.current !== null) {
        window.clearTimeout(resultTimeout.current)
        resultTimeout.current = null
      }
    }

    const stopConnection = () => {
      intentionalStop = true
      clearResultTimeout()
      if (activeConnection.current === connection) {
        activeConnection.current = null
      }

      void connection.stop().catch((stopError: unknown) => {
        console.error('Could not stop the training result connection.', stopError)
      })
    }

    setError('')
    setDebrief(null)
    setStatus('Connecting to the training service…')
    setIsSubmitting(true)
    activeConnection.current = connection

    connection.on('TrainingDebriefCompleted', (result: TrainingDebrief) => {
      if (result.simulationId !== simulationId) {
        return
      }

      clearResultTimeout()
      receivedResult = true
      setDebrief(result)
      setIsWaitingForDebrief(false)
      setStatus('Debrief complete')
      stopConnection()
    })
    connection.onreconnected(async () => {
      try {
        await connection.invoke('JoinSimulation', simulationId)
      } catch (reconnectError) {
        setError(
          reconnectError instanceof Error
            ? reconnectError.message
            : 'Could not reconnect to the training result stream.',
        )
      }
    })
    connection.onclose((closeError) => {
      if (!receivedResult && !intentionalStop) {
        clearResultTimeout()
        setIsWaitingForDebrief(false)
        setError(closeError?.message ?? 'The training result connection closed unexpectedly.')
      }
    })

    try {
      await connection.start()
      await connection.invoke('JoinSimulation', simulationId)

      resultTimeout.current = window.setTimeout(() => {
        if (!receivedResult) {
          timedOut = true
          setIsWaitingForDebrief(false)
          setError('No debrief arrived within two minutes. Check that the training services and Ollama are running.')
          stopConnection()
        }
      }, 120_000)

      const response = await fetch(`${apiBaseUrl}/api/simulations`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ simulationId, scenarioId, behaviorOptionId }),
      })

      if (!response.ok) {
        throw new Error(await getApiError(response))
      }

      setIsWaitingForDebrief(!receivedResult && !timedOut)
      if (!receivedResult && !timedOut) {
        setStatus('Simulation accepted. Waiting for the AI training debrief…')
      }
    } catch (submissionError) {
      setIsWaitingForDebrief(false)
      setError(
        submissionError instanceof Error
          ? submissionError.message
          : 'Could not start the simulation.',
      )
      setStatus('')
      stopConnection()
    } finally {
      setIsSubmitting(false)
    }
  }

  const isBusy = isLoadingScenarios || isSubmitting || isWaitingForDebrief

  return (
    <main className="app-shell">
      <header className="topbar">
        <a className="brand" href="/" aria-label="CareSim home">
          <span className="brand-mark" aria-hidden="true">C</span>
          <span>CareSim <span className="brand-light">Training</span></span>
        </a>
        <span className="environment-badge"><span /> Simulation environment</span>
      </header>

      <section className="intro">
        <p className="eyebrow">STAFF DEVELOPMENT · PHASE 1</p>
        <h1>Practice makes<br /><span>prepared.</span></h1>
        <p className="intro-copy">
          Explore a simulated hospital scenario, choose an observed staff behavior,
          and review a learning-focused debrief.
        </p>
      </section>

      <section className="workspace" aria-label="Start a training simulation">
        <div className="scenario-panel">
          <div className="panel-heading">
            <div>
              <p className="step-label">01 / SCENARIO</p>
              <h2>Choose a scenario</h2>
            </div>
            <span className="step-icon" aria-hidden="true">✳</span>
          </div>

          {isLoadingScenarios ? (
            <p className="muted">Loading scenarios…</p>
          ) : scenarios.length === 0 ? (
            <p className="muted">No scenarios are available.</p>
          ) : (
            <div className="scenario-list">
              {scenarios.map((scenario) => (
                <button
                  className={`scenario-card ${scenario.id === scenarioId ? 'selected' : ''}`}
                  key={scenario.id}
                  type="button"
                  aria-pressed={scenario.id === scenarioId}
                  onClick={() => selectScenario(scenario.id)}
                  disabled={isBusy}
                >
                  <span className="scenario-symbol" aria-hidden="true">
                    {scenario.id === 'patient-identification' ? '◎' : scenario.id === 'structured-handover' ? '↗' : '⌁'}
                  </span>
                  <span className="scenario-description">
                    <strong>{scenario.name}</strong>
                    <small>{scenario.staffRole} · Simulated training</small>
                  </span>
                  <span className="selection-indicator" aria-hidden="true" />
                </button>
              ))}
            </div>
          )}

          {selectedScenario && (
            <div className="behavior-section">
              <div className="panel-heading behavior-heading">
                <div>
                  <p className="step-label">02 / OBSERVATION</p>
                  <h2>What happened?</h2>
                </div>
              </div>
              <p className="muted">Select the behavior observed in this simulation.</p>
              <div className="behavior-list">
                {selectedScenario.behaviorOptions.map((option) => (
                  <label
                    className={`behavior-option ${behaviorOptionId === option.id ? 'selected' : ''}`}
                    key={option.id}
                  >
                    <input
                      type="radio"
                      name="behavior"
                      value={option.id}
                      checked={behaviorOptionId === option.id}
                      onChange={() => setBehaviorOptionId(option.id)}
                      disabled={isBusy}
                    />
                    <span className="radio-mark" aria-hidden="true" />
                    <span>{option.description}</span>
                  </label>
                ))}
              </div>
              <button
                className="start-button"
                type="button"
                onClick={() => void startSimulation()}
                disabled={isBusy || !behaviorOptionId}
              >
                {isSubmitting ? 'Starting simulation…' : isWaitingForDebrief ? 'Debrief in progress…' : 'Start training review'}
                <span aria-hidden="true">→</span>
              </button>
            </div>
          )}
        </div>

        <aside className="feedback-panel" aria-live="polite">
          <p className="step-label">03 / DEBRIEF</p>
          {debrief ? (
            <div className="debrief-content">
              <span className={`outcome-pill ${debrief.outcome === 'MeetsExpectations' ? 'positive' : 'practice'}`}>
                {debrief.outcome === 'MeetsExpectations' ? 'Meets expectations' : 'Practice recommended'}
              </span>
              <h2>{debrief.scenarioName}</h2>
              <p className="debrief-role">{debrief.staffRole} · {new Date(debrief.completedAt).toLocaleTimeString()}</p>
              <div className="behavior-summary">
                <p><strong>Observed</strong>{debrief.observedBehavior}</p>
                <p><strong>Expected</strong>{debrief.expectedBehavior}</p>
              </div>
              <div className="rubric-feedback">
                <strong>Rule-based rubric</strong>
                <p>
                  {debrief.outcome === 'MeetsExpectations'
                    ? 'The observed behavior matched the expected behavior for this scenario.'
                    : `Practice the expected behavior: ${debrief.expectedBehavior}`}
                </p>
              </div>
              <div className="ai-debrief">
                <div className="ai-label"><span aria-hidden="true">✳</span> AI coaching aid</div>
                <p>{plainTextAiResponse(debrief.aiReflectionQuestion)}</p>
                <span className="ai-disclaimer">Training suggestion — discuss this with an educator</span>
              </div>
              <p className="review-note">Use this as a learning aid. Review feedback with an educator.</p>
            </div>
          ) : (
            <div className="empty-debrief">
              <span className="debrief-icon" aria-hidden="true">✳</span>
              <h2>Your debrief<br />will appear here.</h2>
              <p>{status || 'Complete the scenario to receive a learning-focused review.'}</p>
              {isWaitingForDebrief && <span className="loading-indicator">Processing with Ollama · Phi-3</span>}
            </div>
          )}
          {error && <p className="error-message" role="alert">{error}</p>}
        </aside>
      </section>

      <footer className="footer-note">
        <span className="privacy-icon" aria-hidden="true">◇</span>
        Training-only demo · Do not enter real patient information · Results are not saved
      </footer>
    </main>
  )
}

export default App
