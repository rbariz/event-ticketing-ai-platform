import { useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Bell, Check, ExternalLink } from "lucide-react";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { Button } from "@/components/ui/button";
import { api, type AgentNotification } from "@/lib/api";
import { useI18n } from "@/lib/i18n";
import { useScanDrawer } from "@/components/ScanDrawerProvider";
import { SeverityBadge } from "@/components/badges";
import { cn } from "@/lib/utils";

function fmt(d?: string | null) {
  if (!d) return "—";
  const t = new Date(d);
  return isNaN(t.getTime()) ? "—" : t.toLocaleString();
}

export function NotificationBell() {
  const { t } = useI18n();
  const { openScan } = useScanDrawer();
  const qc = useQueryClient();
  const [open, setOpen] = useState(false);
  const [markErr, setMarkErr] = useState<string | null>(null);

  const q = useQuery({
    queryKey: ["notifications", "unread"],
    queryFn: () => api.agentNotifications({ unreadOnly: true, count: 20 }),
    refetchInterval: 30_000,
  });

  const items: AgentNotification[] = q.data ?? [];
  const unread = items.length;

  const markMut = useMutation({
    mutationFn: (id: string) => api.markNotificationRead(id),
    onSuccess: () => {
      setMarkErr(null);
      qc.invalidateQueries({ queryKey: ["notifications"] });
    },
    onError: () => setMarkErr(t("notif.markFailed")),
  });

  return (
    <Popover open={open} onOpenChange={setOpen}>
      <PopoverTrigger asChild>
        <button
          className="relative inline-flex h-9 w-9 items-center justify-center rounded-md border bg-background text-muted-foreground hover:bg-muted hover:text-foreground transition-colors"
          aria-label={t("notif.title")}
        >
          <Bell className="h-4 w-4" />
          {unread > 0 && (
            <span className="absolute -top-1 -right-1 inline-flex min-w-[18px] h-[18px] items-center justify-center rounded-full bg-red-600 px-1 text-[10px] font-semibold text-white ring-2 ring-background">
              {unread > 99 ? "99+" : unread}
            </span>
          )}
        </button>
      </PopoverTrigger>
      <PopoverContent align="end" className="w-[380px] p-0">
        <div className="flex items-center justify-between border-b px-4 py-2.5">
          <div className="text-sm font-semibold">{t("notif.title")}</div>
          <div className="text-[11px] text-muted-foreground">
            {unread} {t("notif.unread").toLowerCase()}
          </div>
        </div>

        {markErr && (
          <div className="border-b bg-destructive/5 px-4 py-1.5 text-[11px] text-destructive">
            {markErr}
          </div>
        )}

        <div className="max-h-[420px] overflow-y-auto">
          {q.isLoading && (
            <div className="px-4 py-6 text-center text-xs text-muted-foreground">
              {t("common.loading")}
            </div>
          )}
          {q.isError && (
            <div className="px-4 py-6 text-center text-xs text-destructive">
              {t("common.error")}
            </div>
          )}
          {!q.isLoading && !q.isError && items.length === 0 && (
            <div className="px-4 py-8 text-center text-xs text-muted-foreground">
              {t("notif.empty")}
            </div>
          )}
          <ul className="divide-y">
            {items.map((n) => (
              <li
                key={n.id}
                className={cn(
                  "px-4 py-3 text-sm transition-colors",
                  n.isRead ? "bg-background" : "bg-primary/5",
                )}
              >
                <div className="flex items-start justify-between gap-2">
                  <div className="min-w-0 flex-1">
                    <div className="mb-1 flex items-center gap-2">
                      <SeverityBadge severity={n.severity} />
                      <span className="text-[10px] text-muted-foreground">
                        {fmt(n.createdAtUtc)}
                      </span>
                    </div>
                    <div className="font-medium leading-tight">{n.title ?? "—"}</div>
                    <div className="mt-0.5 line-clamp-2 text-xs text-muted-foreground">
                      {n.message ?? ""}
                    </div>
                    <div className="mt-2 flex items-center gap-3">
                      {n.scanAttemptId && (
                        <button
                          onClick={() => {
                            openScan(n.scanAttemptId);
                            setOpen(false);
                          }}
                          className="inline-flex items-center gap-1 text-[11px] font-medium text-primary hover:underline"
                        >
                          <ExternalLink className="h-3 w-3" />
                          {t("notif.openScan")}
                        </button>
                      )}
                      {!n.isRead && (
                        <Button
                          variant="ghost"
                          size="sm"
                          className="h-6 px-2 text-[11px]"
                          disabled={markMut.isPending}
                          onClick={() => markMut.mutate(n.id)}
                        >
                          <Check className="mr-1 h-3 w-3" />
                          {t("notif.markRead")}
                        </Button>
                      )}
                    </div>
                  </div>
                </div>
              </li>
            ))}
          </ul>
        </div>
      </PopoverContent>
    </Popover>
  );
}