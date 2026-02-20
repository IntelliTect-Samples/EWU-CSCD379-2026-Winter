const API = "http://localhost:5237/api"

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

  return await $fetch("http://localhost:5237/api/products/upload", {
    method: "POST",
    body: formData
  })
}