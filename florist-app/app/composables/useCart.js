import { computed } from 'vue'

export const useCart = () => {
  const cart = useState('cart', () => [])

  const addToCart = (product) => {
    cart.value.push({ ...product, cartId: Date.now() })
  }

  const removeFromCart = (cartId) => {
    cart.value = cart.value.filter(item => item.cartId !== cartId)
  }

  const cartTotal = computed(() => {
    return cart.value.reduce((sum, item) => sum + item.price, 0)
  })

  return { cart, addToCart, removeFromCart, cartTotal }
}