import { useApi } from "./useApi";

export function useUser() {
  // Use Nuxt's useState for SSR-safe, global reactivity
  const user = useState<any | null>("user", () => null);
  const isAdmin = useState<boolean>("isAdmin", () => false);
  const loading = useState<boolean>("userLoading", () => true);

  async function fetchUser() {
    loading.value = true;
    try {
      const { apiBase } = useApi();
      const result = await $fetch(`${apiBase.replace("/api", "")}/me`, {
        credentials: "include",
      });
      user.value = result;
      isAdmin.value = !!result?.isAdmin;
    } catch (e) {
      user.value = null;
      isAdmin.value = false;
    } finally {
      loading.value = false;
    }
  }

  return { user, isAdmin, loading, fetchUser };
}
