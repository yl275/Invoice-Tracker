import { useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Copy, UserPlus, LogOut, Plus, RefreshCw } from "lucide-react";
import api from "@/services/api";
import type { TeamDto, TeamMemberDto, TeamInvitationDto } from "@/types";

export default function TeamPage() {
  const [teams, setTeams] = useState<TeamDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [currentTeamId, setCurrentTeamId] = useState<string | null>(null);
  const [members, setMembers] = useState<TeamMemberDto[]>([]);
  const [inviteEmail, setInviteEmail] = useState("");
  const [inviting, setInviting] = useState(false);
  const [lastInvite, setLastInvite] = useState<TeamInvitationDto | null>(null);
  const [copySuccess, setCopySuccess] = useState(false);
  const [createName, setCreateName] = useState("");
  const [creating, setCreating] = useState(false);
  const [leaving, setLeaving] = useState(false);
  const [syncing, setSyncing] = useState(false);

  const [dataScope, setDataScopeState] = useState<"team" | "mine">(() => {
    if (typeof localStorage === "undefined") return "team";
    return (localStorage.getItem("dataScope") as "team" | "mine") || "team";
  });

  const setDataScope = (scope: "team" | "mine") => {
    localStorage.setItem("dataScope", scope);
    setDataScopeState(scope);
    window.dispatchEvent(new CustomEvent("dataScopeChange", { detail: scope }));
  };

  const fetchTeams = () => {
    return api
      .get<TeamDto[]>("/teams")
      .then((res) => {
        const list = res.data ?? [];
        setTeams(list);
        const stored = typeof localStorage !== "undefined" ? localStorage.getItem("currentTeamId") : null;
        if (list.length > 0 && stored && list.some((t) => t.id === stored)) {
          setCurrentTeamId(stored);
        } else if (list.length > 0 && (!currentTeamId || !list.some((t) => t.id === currentTeamId))) {
          const next = list[0].id;
          setCurrentTeamId(next);
          if (typeof localStorage !== "undefined") localStorage.setItem("currentTeamId", next);
        }
      })
      .catch((err) => console.error("Failed to load teams", err))
      .finally(() => setLoading(false));
  };

  const handleSync = async () => {
    setSyncing(true);
    setLoading(true);
    try {
      const res = await api.get<TeamDto[]>("/teams");
      const list = res.data ?? [];
      setTeams(list);
      const stored = typeof localStorage !== "undefined" ? localStorage.getItem("currentTeamId") : null;
      if (list.length > 0 && stored && list.some((t) => t.id === stored)) {
        setCurrentTeamId(stored);
        if (typeof localStorage !== "undefined") localStorage.setItem("currentTeamId", stored);
      } else if (list.length > 0) {
        const next = list[0].id;
        setCurrentTeamId(next);
        if (typeof localStorage !== "undefined") localStorage.setItem("currentTeamId", next);
      } else if (typeof localStorage !== "undefined") {
        localStorage.removeItem("currentTeamId");
        setCurrentTeamId(null);
      }
      window.dispatchEvent(new CustomEvent("teamMembershipChange"));
    } catch (err) {
      console.error("Sync failed", err);
      alert("Sync failed. Please try again.");
    } finally {
      setSyncing(false);
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchTeams();
  }, []);

  const handleCreateTeam = async () => {
    if (!createName.trim() || creating) return;
    setCreating(true);
    try {
      const res = await api.post<TeamDto>("/teams", { name: createName.trim() });
      setCreateName("");
      await fetchTeams();
      if (res.data?.id) {
        setCurrentTeamId(res.data.id);
        if (typeof localStorage !== "undefined") localStorage.setItem("currentTeamId", res.data.id);
      }
    } catch (err) {
      console.error("Create team failed", err);
      alert("Failed to create team. Please try again.");
    } finally {
      setCreating(false);
    }
  };

  const handleLeaveTeam = async () => {
    if (!currentTeamId || leaving) return;
    if (!confirm("Leave this team? You will lose access to its shared clients, products, and invoices."))
      return;
    const leftTeamId = currentTeamId;
    setLeaving(true);
    try {
      await api.post(`/teams/${currentTeamId}/leave`);
      if (typeof localStorage !== "undefined" && localStorage.getItem("currentTeamId") === leftTeamId)
        localStorage.removeItem("currentTeamId");
      window.dispatchEvent(new CustomEvent("teamMembershipChange"));
      await fetchTeams();
      setCurrentTeamId(null);
    } catch (err: unknown) {
      console.error("Leave team failed", err);
      const data = err && typeof err === "object" && "response" in err
        ? (err as { response?: { data?: unknown } }).response?.data
        : undefined;
      const message = typeof data === "string" ? data : "Failed to leave team. You cannot leave your last workspace.";
      alert(message);
    } finally {
      setLeaving(false);
    }
  };

  useEffect(() => {
    if (!currentTeamId) {
      setMembers([]);
      return;
    }
    api
      .get<TeamMemberDto[]>(`/teams/${currentTeamId}/members`)
      .then((res) => setMembers(res.data ?? []))
      .catch((err) => console.error("Failed to load members", err));
  }, [currentTeamId]);

  const currentTeam = teams.find((t) => t.id === currentTeamId);

  const handleInvite = async () => {
    if (!currentTeamId || !inviteEmail.trim()) return;
    setInviting(true);
    setLastInvite(null);
    try {
      const res = await api.post<TeamInvitationDto>(`/teams/${currentTeamId}/invite`, {
        email: inviteEmail.trim(),
      });
      setLastInvite(res.data);
      setInviteEmail("");
    } catch (err) {
      console.error("Invite failed", err);
      alert("Failed to create invite. Please try again.");
    } finally {
      setInviting(false);
    }
  };

  const copyInviteLink = (link: string) => {
    navigator.clipboard.writeText(link).then(() => {
      setCopySuccess(true);
      setTimeout(() => setCopySuccess(false), 2000);
    });
  };

  return (
    <div className="container max-w-2xl mx-auto py-10">
      <div className="mb-6">
        <h1 className="text-3xl font-bold tracking-tight">Team</h1>
        <p className="text-muted-foreground">
          Invite members to share clients, products, and invoices.
        </p>
      </div>

      <div className="mb-6 flex flex-wrap items-end gap-4">
        {teams.length > 0 && (
          <div className="flex items-end gap-2">
            <div>
              <label className="text-sm font-medium mb-2 block">Current workspace</label>
              <select
                className="w-full max-w-xs rounded-md border bg-background px-3 py-2 text-sm"
                value={currentTeamId ?? ""}
                onChange={(e) => {
                  const id = e.target.value || null;
                  setCurrentTeamId(id);
                  if (typeof localStorage !== "undefined") {
                    if (id) localStorage.setItem("currentTeamId", id);
                    else localStorage.removeItem("currentTeamId");
                  }
                }}
              >
                {teams.map((t) => (
                  <option key={t.id} value={t.id}>
                    {t.name}
                  </option>
                ))}
              </select>
            </div>
            <Button variant="outline" size="icon" onClick={handleSync} disabled={syncing} title="Sync workspaces and refresh lists">
              <RefreshCw className={`h-4 w-4 ${syncing ? "animate-spin" : ""}`} />
            </Button>
          </div>
        )}
        <div className="flex items-end gap-2">
          <Input
            placeholder="New team name"
            value={createName}
            onChange={(e) => setCreateName(e.target.value)}
            onKeyDown={(e) => e.key === "Enter" && handleCreateTeam()}
            className="w-48"
          />
          <Button onClick={handleCreateTeam} disabled={creating || !createName.trim()}>
            <Plus className="h-4 w-4 mr-1" />
            {creating ? "Creating…" : "Create team"}
          </Button>
        </div>
      </div>

      <div className="mb-6 flex flex-wrap items-center gap-3 rounded-lg border bg-muted/30 p-4">
        <span className="text-sm font-medium">Show in Clients / Products / Invoices:</span>
        <div className="inline-flex rounded-md border bg-background p-0.5">
          <button
            type="button"
            onClick={() => setDataScope("team")}
            className={`rounded px-3 py-1.5 text-sm font-medium transition-colors ${
              dataScope === "team"
                ? "bg-primary text-primary-foreground"
                : "text-muted-foreground hover:text-foreground"
            }`}
          >
            All team
          </button>
          <button
            type="button"
            onClick={() => setDataScope("mine")}
            className={`rounded px-3 py-1.5 text-sm font-medium transition-colors ${
              dataScope === "mine"
                ? "bg-primary text-primary-foreground"
                : "text-muted-foreground hover:text-foreground"
            }`}
          >
            Only mine
          </button>
        </div>
        <span className="text-xs text-muted-foreground">
          {dataScope === "team"
            ? "Lists show all members’ items."
            : "Lists show only items you created. To see teammates’ invoices, clients, and products, switch to “All team”."}
        </span>
      </div>

      {currentTeam && (
        <>
          <div className="rounded-lg border bg-card p-6 space-y-4">
            <h2 className="text-lg font-semibold">{currentTeam.name}</h2>

            <div className="space-y-2">
              <p className="text-sm font-medium">Members</p>
              <ul className="space-y-2">
                {members.map((m) => (
                  <li
                    key={m.userId}
                    className="flex items-center justify-between rounded-md border px-3 py-2 text-sm"
                  >
                    <span className="font-mono text-muted-foreground truncate mr-2">
                      {m.userId}
                    </span>
                    <span className="text-xs text-muted-foreground capitalize">
                      {m.role.toLowerCase()}
                    </span>
                  </li>
                ))}
              </ul>
            </div>

            <div className="pt-4 border-t space-y-2">
              <p className="text-sm font-medium">Invite by email</p>
              <div className="flex gap-2">
                <Input
                  type="email"
                  placeholder="colleague@example.com"
                  value={inviteEmail}
                  onChange={(e) => setInviteEmail(e.target.value)}
                  onKeyDown={(e) => e.key === "Enter" && handleInvite()}
                />
                <Button onClick={handleInvite} disabled={inviting}>
                  <UserPlus className="h-4 w-4 mr-1" />
                  {inviting ? "Sending…" : "Invite"}
                </Button>
              </div>
              {lastInvite && (
                <div className="rounded-md bg-muted/50 p-3 text-sm space-y-2">
                  <p className="text-muted-foreground">
                    Share this link with <strong>{lastInvite.email}</strong> (expires{" "}
                    {new Date(lastInvite.expiresAt).toLocaleDateString()}):
                  </p>
                  <div className="flex gap-2">
                    <Input
                      readOnly
                      value={lastInvite.inviteLink}
                      className="font-mono text-xs"
                    />
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={() => copyInviteLink(lastInvite.inviteLink)}
                    >
                      <Copy className="h-4 w-4 mr-1" />
                      {copySuccess ? "Copied!" : "Copy"}
                    </Button>
                  </div>
                </div>
              )}
            </div>

            <div className="pt-4 border-t">
              <Button
                type="button"
                variant="outline"
                size="sm"
                className="text-destructive hover:bg-destructive/10 hover:text-destructive"
                onClick={handleLeaveTeam}
                disabled={leaving}
              >
                <LogOut className="h-4 w-4 mr-1" />
                {leaving ? "Leaving…" : "Leave team"}
              </Button>
            </div>
          </div>
        </>
      )}

      {loading && teams.length === 0 && (
        <p className="text-muted-foreground">Loading teams…</p>
      )}

      {!loading && teams.length === 0 && (
        <div className="rounded-lg border bg-card p-6 space-y-4">
          <p className="text-muted-foreground">You have no teams yet. Create one to get started.</p>
          <div className="flex gap-2">
            <Input
              placeholder="Team name"
              value={createName}
              onChange={(e) => setCreateName(e.target.value)}
              onKeyDown={(e) => e.key === "Enter" && handleCreateTeam()}
              className="max-w-xs"
            />
            <Button onClick={handleCreateTeam} disabled={creating || !createName.trim()}>
              <Plus className="h-4 w-4 mr-1" />
              {creating ? "Creating…" : "Create team"}
            </Button>
          </div>
        </div>
      )}
    </div>
  );
}
