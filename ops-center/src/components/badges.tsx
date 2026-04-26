import { cn } from "@/lib/utils";

export function RiskBadge({ level }: { level?: string | null }) {
  if (!level) return <span className="text-muted-foreground text-xs">—</span>;
  const l = level.toLowerCase();
  const styles: Record<string, string> = {
    low: "bg-[var(--risk-low-bg)] text-[var(--risk-low)] ring-[var(--risk-low)]/20",
    medium: "bg-[var(--risk-medium-bg)] text-[var(--risk-medium)] ring-[var(--risk-medium)]/30",
    high: "bg-[var(--risk-high-bg)] text-[var(--risk-high)] ring-[var(--risk-high)]/30",
    critical: "bg-[var(--risk-critical-bg)] text-[var(--risk-critical)] ring-[var(--risk-critical)]/30",
  };
  return (
    <span
      className={cn(
        "inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium ring-1 ring-inset",
        styles[l] ?? "bg-muted text-muted-foreground ring-border",
      )}
    >
      {level}
    </span>
  );
}

export function ProviderBadge({ provider }: { provider?: string | null }) {
  if (!provider) return <span className="text-muted-foreground text-xs">—</span>;
  const p = provider.toLowerCase();
  const styles: Record<string, string> = {
    openai: "bg-[var(--provider-openai-bg)] text-[var(--provider-openai)] ring-[var(--provider-openai)]/25",
    rulebased: "bg-[var(--provider-rule-bg)] text-[var(--provider-rule)] ring-[var(--provider-rule)]/20",
    fallback: "bg-[var(--provider-fallback-bg)] text-[var(--provider-fallback)] ring-[var(--provider-fallback)]/30",
  };
  return (
    <span
      className={cn(
        "inline-flex items-center rounded-md px-2 py-0.5 text-xs font-medium ring-1 ring-inset",
        styles[p] ?? "bg-muted text-muted-foreground ring-border",
      )}
    >
      {provider}
    </span>
  );
}

export function DecisionBadge({ decision }: { decision?: string | null }) {
  if (!decision) return <span className="text-muted-foreground text-xs">—</span>;
  const d = decision.toLowerCase();
  const styles: Record<string, string> = {
    accepted: "bg-emerald-50 text-emerald-700 ring-emerald-600/20",
    accept: "bg-emerald-50 text-emerald-700 ring-emerald-600/20",
    valid: "bg-emerald-50 text-emerald-700 ring-emerald-600/20",
    rejected: "bg-red-50 text-red-700 ring-red-600/20",
    reject: "bg-red-50 text-red-700 ring-red-600/20",
    invalid: "bg-red-50 text-red-700 ring-red-600/20",
    duplicate: "bg-amber-50 text-amber-700 ring-amber-600/20",
    expired: "bg-zinc-100 text-zinc-700 ring-zinc-600/20",
    alreadyused: "bg-orange-50 text-orange-700 ring-orange-600/20",
    already_used: "bg-orange-50 text-orange-700 ring-orange-600/20",
    pending: "bg-blue-50 text-blue-700 ring-blue-600/20",
  };
  const key = d.replace(/[\s-]/g, "");
  return (
    <span
      className={cn(
        "inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium ring-1 ring-inset capitalize",
        styles[key] ?? "bg-muted text-muted-foreground ring-border",
      )}
    >
      {decision}
    </span>
  );
}