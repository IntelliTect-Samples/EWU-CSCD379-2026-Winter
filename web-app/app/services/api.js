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
  const token = localStorage.getItem("accessToken")
  
  return $fetch(`${API}/orders`, {
    method: "POST",
    body: order,
    headers: {
      Authorization: `Bearer ${token}`
    }
  })
}

export const addProduct = (product) => {
  const API = getApiBase()
  const token = localStorage.getItem("accessToken")
  
  return $fetch(`${API}/products`, {
    method: "POST",
    body: product,
    headers: {
      Authorization: `Bearer ${token}`
    }
  })
}

export const uploadImage = async (file) => {
  const API = getApiBase()
  const token = localStorage.getItem("accessToken")

  const formData = new FormData()
  formData.append("file", file)

  return await $fetch(`${API}/products/upload`, {
    method: "POST",
    body: formData,
    headers: {
      Authorization: `Bearer ${token}`
    }
  })
}

export const deleteProduct = (id) => {
  const API = getApiBase()
  const token = localStorage.getItem("accessToken")
  return $fetch(`${API}/products/${id}`, {
    method: "DELETE",
    headers: {
      Authorization: `Bearer ${token}`
    }
  })
}

export const deleteOrder = async (id) => {
  const API = getApiBase()
  const token = localStorage.getItem("accessToken")

  return await $fetch(`${API}/orders/${id}`, {
    method: 'DELETE',
    headers: {
      Authorization: `Bearer ${token}`
    }
  })
}