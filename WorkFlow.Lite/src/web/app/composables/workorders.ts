import { useApi } from './api'

export function useWorkOrdersService() {
  const { apiFetch } = useApi()
  return {
    getPublicBoard: () => apiFetch('/api/public/board'),
    getMine: () => apiFetch('/api/workorders/mine'),
    create: (payload: { title: string; description: string; priority: string }) =>
      apiFetch('/api/workorders', { method: 'POST', body: payload })
  }
}