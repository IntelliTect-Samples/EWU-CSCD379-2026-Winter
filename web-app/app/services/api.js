const config = useRuntimeConfig()

const API = config.public.apiBase + "/api"

export const getProducts = () => {
  return $fetch(`${API}/products`)
}

export const createOrder = (order) => {
  return $fetch(`${API}/orders`, {
    method: "POST",
    body: order
  })
}

export const addProduct = (product) => {
  return $fetch(`${API}/products`, {
    method: "POST",
    body: product
  })
}

export const uploadImage = async (file) => {
  const formData = new FormData()
  formData.append("file", file)

  return await $fetch(`${API}/products/upload`, {
    method: "POST",
    body: formData
  })
}

export const deleteProduct = (id) => {
  return $fetch(`${API}/products/${id}`, {
    method: "DELETE"
  })
}