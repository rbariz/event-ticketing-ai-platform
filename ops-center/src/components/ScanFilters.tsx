import { useState, useEffect } from "react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { useI18n } from "@/lib/i18n";
import type { ScanFilters as Filters } from "@/lib/api";

interface Props {
  value: Filters;
  onChange: (f: Filters) => void;
}

export function ScanFiltersBar({ value, onChange }: Props) {
  const { t } = useI18n();
  const [local, setLocal] = useState<Filters>(value);

  useEffect(() => setLocal(value), [value]);

  const update = (k: keyof Filters, v: string) =>
    setLocal((p) => ({ ...p, [k]: v || undefined }));

  return (
    <div className="rounded-lg border bg-card p-4 shadow-sm">
      <div className="grid grid-cols-1 gap-3 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-6">
        <Field label={t("common.from")}>
          <Input
            type="datetime-local"
            value={local.fromUtc ?? ""}
            onChange={(e) => update("fromUtc", e.target.value)}
          />
        </Field>
        <Field label={t("common.to")}>
          <Input
            type="datetime-local"
            value={local.toUtc ?? ""}
            onChange={(e) => update("toUtc", e.target.value)}
          />
        </Field>
        <Field label={t("common.gate")}>
          <Input
            placeholder="gate-01"
            value={local.gateId ?? ""}
            onChange={(e) => update("gateId", e.target.value)}
          />
        </Field>
        <Field label={t("common.source")}>
          <Input
            placeholder="mobile / kiosk"
            value={local.source ?? ""}
            onChange={(e) => update("source", e.target.value)}
          />
        </Field>
        <Field label={t("common.decision")}>
          <Input
            placeholder="Accepted / Rejected"
            value={local.decision ?? ""}
            onChange={(e) => update("decision", e.target.value)}
          />
        </Field>
        <Field label={t("common.reason")}>
          <Input
            placeholder="DUPLICATE_SCAN"
            value={local.reasonCode ?? ""}
            onChange={(e) => update("reasonCode", e.target.value)}
          />
        </Field>
      </div>
      <div className="mt-3 flex items-center justify-end gap-2">
        <Button
          variant="ghost"
          size="sm"
          onClick={() => {
            setLocal({});
            onChange({});
          }}
        >
          {t("common.reset")}
        </Button>
        <Button size="sm" onClick={() => onChange(local)}>
          {t("common.apply")}
        </Button>
      </div>
    </div>
  );
}

function Field({ label, children }: { label: string; children: React.ReactNode }) {
  return (
    <div className="space-y-1.5">
      <Label className="text-xs font-medium text-muted-foreground">{label}</Label>
      {children}
    </div>
  );
}