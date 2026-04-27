import { createContext, useContext, useState, type ReactNode } from "react";
import { ScanDetailDrawer } from "@/components/ScanDetailDrawer";

interface Ctx {
  openScan: (id: string) => void;
}

const ScanDrawerCtx = createContext<Ctx | null>(null);

export function ScanDrawerProvider({ children }: { children: ReactNode }) {
  const [openId, setOpenId] = useState<string | null>(null);
  return (
    <ScanDrawerCtx.Provider value={{ openScan: (id) => setOpenId(id) }}>
      {children}
      <ScanDetailDrawer
        scanId={openId}
        open={!!openId}
        onOpenChange={(o) => !o && setOpenId(null)}
      />
    </ScanDrawerCtx.Provider>
  );
}

export function useScanDrawer() {
  const ctx = useContext(ScanDrawerCtx);
  if (!ctx) throw new Error("useScanDrawer must be used within ScanDrawerProvider");
  return ctx;
}