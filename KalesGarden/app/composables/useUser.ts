import { useApi } from "./useApi";
import { useAuth } from "./useAuth";

export function useUser() {
  // Use Nuxt's useState for SSR-safe, global reactivity
  const user = useState<any | null>("user", () => null);
  const isAdmin = useState<boolean>("isAdmin", () => false);
  const loading = useState<boolean>("userLoading", () => true);

  async function fetchUser() {
    loading.value = true;
    try {
      const { apiFetch } = useApi();
      const result = await apiFetch<{
        id: string;
        email: string;
        roles: string[];
      }>("/auth/me");
      user.value = result;
      isAdmin.value = result?.roles?.includes("Admin") ?? false;
    } catch (e) {
      user.value = null;
      isAdmin.value = false;
    } finally {
      loading.value = false;
    }
  }

  return { user, isAdmin, loading, fetchUser };
}
