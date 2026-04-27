import type { AgentDecisionLog } from "@/lib/api";
import { useScanDrawer } from "@/components/ScanDrawerProvider";
import { SeverityBadge, RiskBadge, ActionBadge, ProviderBadge } from "@/components/badges";
import { useI18n } from "@/lib/i18n";
import { AlertTriangle } from "lucide-react";

function fmt(d?: string | null) {
  if (!d) return "—";
  const t = new Date(d);
  return isNaN(t.getTime()) ? "—" : t.toLocaleString();
}

export function AgentDecisionList({
  logs,
  compact = false,
}: {
  logs: AgentDecisionLog[];
  compact?: boolean;
}) {
  const { t } = useI18n();
  const { openScan } = useScanDrawer();
  const list = logs ?? [];

  return (
    <ul className="divide-y">
      {list.map((d) => {
        const actions = d.actions ?? [];
        return (
          <li
            key={d.id}
            onClick={() => d.scanAttemptId && openScan(d.scanAttemptId)}
            className="cursor-pointer px-4 py-3 transition-colors hover:bg-muted/50"
          >
            <div className="flex flex-wrap items-center gap-2 text-xs">
              <span className="text-muted-foreground tabular-nums">
                {fmt(d.createdAtUtc)}
              </span>
              <SeverityBadge severity={d.severity} />
              <RiskBadge level={d.riskLevel} />
              <span className="font-mono text-[11px] text-muted-foreground">
                {d.riskScore != null ? d.riskScore.toFixed(2) : "—"}
              </span>
              {d.requiresHumanReview && (
                <span className="inline-flex items-center gap-1 rounded-full bg-amber-50 px-2 py-0.5 text-[10px] font-medium text-amber-800 ring-1 ring-inset ring-amber-600/30">
                  <AlertTriangle className="h-3 w-3" />
                  {t("agent.requiresReview")}
                </span>
              )}
              {!compact && <ProviderBadge provider={d.provider} />}
            </div>

            {actions.length > 0 && (
              <div className="mt-1.5 flex flex-wrap gap-1">
                {actions.map((a, i) => (
                  <ActionBadge key={`${a}-${i}`} action={a} />
                ))}
              </div>
            )}

            {!compact && d.reason && (
              <div className="mt-1.5 text-xs text-muted-foreground line-clamp-2">
                {d.reason}
              </div>
            )}
          </li>
        );
      })}
    </ul>
  );
}