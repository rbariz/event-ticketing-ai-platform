const BASE = (import.meta.env.VITE_API_BASE_URL ?? "").replace(/\/$/, "");

export class ApiError extends Error {
  constructor(message: string, public status?: number) {
    super(message);
  }
}

export async function apiFetch<T>(path: string, init?: RequestInit): Promise<T> {
  const url = `${BASE}${path}`;
  let res: Response;
  try {
    res = await fetch(url, {
      ...init,
      headers: {
        "Content-Type": "application/json",
        Accept: "application/json",
        ...(init?.headers ?? {}),
      },
    });
  } catch (e) {
    throw new ApiError(`Network error reaching ${url}`);
  }
  if (!res.ok) {
    let detail = "";
    try {
      const t = await res.text();
      detail = t.slice(0, 200);
    } catch {}
    throw new ApiError(
      `Request failed (${res.status}) ${detail}`.trim(),
      res.status,
    );
  }
  if (res.status === 204) return undefined as T;
  return (await res.json()) as T;
}

export type ScanFilters = {
  fromUtc?: string;
  toUtc?: string;
  gateId?: string;
  source?: string;
  decision?: string;
  reasonCode?: string;
  ticketCode?: string;
};

export function buildQuery(params: Record<string, string | number | undefined | null>) {
  const sp = new URLSearchParams();
  for (const [k, v] of Object.entries(params)) {
    if (v === undefined || v === null || v === "") continue;
    sp.set(k, String(v));
  }
  const s = sp.toString();
  return s ? `?${s}` : "";
}

/* ---------------- Types ---------------- */
export interface DashboardSummary {
  totalScans: number;
  acceptedScans: number;
  rejectedScans: number;
  duplicateScans: number;
  expiredScans: number;
  alreadyUsedScans: number;
  highRiskScans: number;
  topRejectReasons: Array<{ reasonCode: string; count: number }>;
  topGates: Array<{ gateId: string; count: number }>;
  recentScans?: Scan[];
}

export interface Scan {
  id: string;
  scannedAtUtc: string;
  ticketCode: string;
  decision: string;
  reasonCode?: string | null;
  gateId?: string | null;
  deviceId?: string | null;
  source?: string | null;
  riskScore?: number | null;
  riskLevel?: string | null;
}

export interface RiskAnalysis {
  riskScore?: number;
  riskLevel?: "Low" | "Medium" | "High" | "Critical" | string;
  recommendedAction?: string;
  explanationSummary?: string;
  explanationConfidence?: number;
  explanationProvider?: "OpenAI" | "RuleBased" | "Fallback" | string;
  riskExplanation?: string;
  riskSignals?: Array<{ name?: string; description?: string; weight?: number } | string>;
}

export interface Ticket {
  code: string;
  status?: string;
  eventName?: string;
  eventDate?: string;
  holderName?: string;
  seat?: string;
  category?: string;
  price?: number;
  currency?: string;
  issuedAt?: string;
  [k: string]: unknown;
}

export interface AgentNotification {
  id: string;
  scanAttemptId: string;
  severity: string;
  title: string;
  message: string;
  isRead: boolean;
  createdAtUtc: string;
  readAtUtc: string | null;
}

export interface AgentDecisionLog {
  id: string;
  scanAttemptId: string;
  riskScore: number;
  riskLevel: string;
  severity: string;
  actions: string[];
  reason: string;
  requiresHumanReview: boolean;
  provider: string;
  createdAtUtc: string;
  operatorSummary?: string | null;
  suggestedNextActions?: string[] | null;
  confidenceScore?: number | null;
  businessImpact?: string | null;
  enrichmentProvider?: string | null;
}

export interface AgentEnrichment {
  operatorSummary?: string;
  suggestedNextActions?: string[];
  confidenceScore?: number;
  businessImpact?: string;
  provider?: "OpenAI" | "RuleBased" | string;
}

export interface AgentDecisionResponse {
  severity?: string;
  actions?: string[];
  reason?: string;
  requiresHumanReview?: boolean;
  enrichment?: AgentEnrichment;
}

export type IncidentStatus = "Open" | "InProgress" | "Resolved" | string;
export type IncidentSeverity = "Low" | "Medium" | "High" | "Critical" | string;

export interface Incident {
  id: string;
  scanAttemptId: string;
  severity: IncidentSeverity;
  status: IncidentStatus;
  title: string;
  description: string;
  assignedTo: string | null;
  createdAtUtc: string;
  assignedAtUtc: string | null;
  resolvedAtUtc: string | null;
  resolutionNote: string | null;
}

export interface IncidentFilters {
  status?: string;
  severity?: string;
  count?: number;
}

/* ---------------- Calls ---------------- */
export const api = {
  dashboardSummary: (filters: ScanFilters = {}) =>
    apiFetch<DashboardSummary>(`/api/dashboard/summary${buildQuery(filters)}`),
  scans: (filters: ScanFilters = {}) =>
    apiFetch<Scan[]>(`/api/scans${buildQuery(filters)}`),
  recentScans: (count = 20) =>
    apiFetch<Scan[]>(`/api/scans/recent${buildQuery({ count })}`),
  scan: (id: string) => apiFetch<Scan>(`/api/scans/${encodeURIComponent(id)}`),
  risk: (id: string, lang: "en" | "fr") =>
    apiFetch<RiskAnalysis>(`/api/scans/${encodeURIComponent(id)}/risk${buildQuery({ lang })}`),
  ticketByCode: (code: string) =>
    apiFetch<Ticket>(`/api/tickets/by-code/${encodeURIComponent(code)}`),
  validateScan: (body: { ticketCode: string; deviceId?: string; gateId?: string; source?: string }) =>
    apiFetch<Scan & RiskAnalysis>(`/api/scans/validate`, {
      method: "POST",
      body: JSON.stringify(body),
    }),
  agentNotifications: (opts: { unreadOnly?: boolean; count?: number } = {}) =>
    apiFetch<AgentNotification[]>(
      `/api/agent/notifications${buildQuery({
        unreadOnly: opts.unreadOnly ? "true" : undefined,
        count: opts.count,
      })}`,
    ),
  markNotificationRead: (id: string) =>
    apiFetch<void>(`/api/agent/notifications/${encodeURIComponent(id)}/mark-read`, {
      method: "POST",
    }),
  agentDecisionLogs: (count = 20) =>
    apiFetch<AgentDecisionLog[]>(`/api/agent/decision-logs${buildQuery({ count })}`),
  analyzeScan: (scanId: string, lang: "en" | "fr") =>
    apiFetch<AgentDecisionResponse>(
      `/api/agent/analyze-scan/${encodeURIComponent(scanId)}${buildQuery({ lang })}`,
      { method: "POST" },
    ),
  incidents: (filters: IncidentFilters = {}) =>
    apiFetch<Incident[]>(`/api/incidents${buildQuery(filters as Record<string, string | number | undefined>)}`),
  incident: (id: string) =>
    apiFetch<Incident>(`/api/incidents/${encodeURIComponent(id)}`),
  assignIncident: (id: string, assignedTo: string) =>
    apiFetch<Incident>(`/api/incidents/${encodeURIComponent(id)}/assign`, {
      method: "POST",
      body: JSON.stringify({ assignedTo }),
    }),
  resolveIncident: (id: string, resolutionNote: string) =>
    apiFetch<Incident>(`/api/incidents/${encodeURIComponent(id)}/resolve`, {
      method: "POST",
      body: JSON.stringify({ resolutionNote }),
    }),
};