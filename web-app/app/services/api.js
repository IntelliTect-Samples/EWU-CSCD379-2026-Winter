export const getApiBase = () => {
  const config = useRuntimeConfig()
  return config.public.apiBase + "/api"
}

export const getProducts = () => {
  const API = getApiBase()
  return $fetch(`${API}/products`)
}

export const createOrder = (order) => {
  const API = getApiBase()
  return $fetch(`${API}/orders`, {
    method: "POST",
    body: order
  })
}

export const addProduct = (product) => {
  const API = getApiBase()
  return $fetch(`${API}/products`, {
    method: "POST",
    body: product
  })
}

export const uploadImage = async (file) => {
  const API = getApiBase()

  const formData = new FormData()
  formData.append("file", file)

  return await $fetch(`${API}/products/upload`, {
    method: "POST",
    body: formData
  })
}

export const deleteProduct = (id) => {
  const API = getApiBase()
  return $fetch(`${API}/products/${id}`, {
    method: "DELETE"
  })
}

export const deleteOrder = async (id) => {
  return await $fetch(`/api/orders/${id}`, {
    method: 'DELETE'
  })
}