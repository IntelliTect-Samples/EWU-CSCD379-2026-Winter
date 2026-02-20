const API = "http://localhost:5000/api"

export const getProducts = () => {
  return $fetch(`${API}/products`)
}

export const createOrder = (order) => {
  return $fetch(`${API}/orders`, {
    method: "POST",
    body: order
  })
}