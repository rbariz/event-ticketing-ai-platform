import { createContext, useContext, useState, type ReactNode } from "react";
import { IncidentDetailDrawer } from "@/components/IncidentDetailDrawer";

interface Ctx {
  openIncident: (id: string) => void;
}

const IncidentDrawerCtx = createContext<Ctx | null>(null);

export function IncidentDrawerProvider({ children }: { children: ReactNode }) {
  const [openId, setOpenId] = useState<string | null>(null);
  return (
    <IncidentDrawerCtx.Provider value={{ openIncident: (id) => setOpenId(id) }}>
      {children}
      <IncidentDetailDrawer
        incidentId={openId}
        open={!!openId}
        onOpenChange={(o) => !o && setOpenId(null)}
      />
    </IncidentDrawerCtx.Provider>
  );
}

export function useIncidentDrawer() {
  const ctx = useContext(IncidentDrawerCtx);
  if (!ctx) throw new Error("useIncidentDrawer must be used within IncidentDrawerProvider");
  return ctx;
}