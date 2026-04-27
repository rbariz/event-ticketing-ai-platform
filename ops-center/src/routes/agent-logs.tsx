import { createFileRoute } from "@tanstack/react-router";
import { useQuery } from "@tanstack/react-query";
import { api } from "@/lib/api";
import { useI18n } from "@/lib/i18n";
import { LoadingBlock, ErrorBlock, EmptyBlock } from "@/components/states";
import { AgentDecisionList } from "@/components/AgentDecisionList";

export const Route = createFileRoute("/agent-logs")({
  component: AgentLogsPage,
});

function AgentLogsPage() {
  const { t } = useI18n();
  const q = useQuery({
    queryKey: ["agent", "decision-logs", 50],
    queryFn: () => api.agentDecisionLogs(50),
  });

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold tracking-tight">{t("agent.logsTitle")}</h1>
        <p className="text-sm text-muted-foreground">{t("agent.logsSubtitle")}</p>
      </div>

      <section className="rounded-lg border bg-card shadow-sm">
        {q.isLoading && <LoadingBlock />}
        {q.isError && (
          <div className="p-4">
            <ErrorBlock error={q.error} onRetry={() => q.refetch()} />
          </div>
        )}
        {q.data &&
          ((q.data ?? []).length ? (
            <AgentDecisionList logs={q.data ?? []} />
          ) : (
            <EmptyBlock />
          ))}
      </section>
    </div>
  );
}