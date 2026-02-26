import { computed, watch, onMounted } from 'vue'

export const useCart = () => {
  const cart = useState('cart', () => [])

  onMounted(() => {
    const savedCart = localStorage.getItem('bloom_and_stem_cart')
    if (savedCart) {
      cart.value = JSON.parse(savedCart)
    }
  })

  watch(cart, (newCart) => {
    localStorage.setItem('bloom_and_stem_cart', JSON.stringify(newCart))
  }, { deep: true })

  const addToCart = (product) => {
    const existingItem = cart.value.find(item => item.id === product.id)
    if (existingItem) {
      existingItem.quantity++
    } else {
      cart.value.push({ ...product, quantity: 1 })
    }
  }

  const removeFromCart = (productId) => {
    const existingItem = cart.value.find(item => item.id === productId)
    if (existingItem && existingItem.quantity > 1) {
      existingItem.quantity--
    } else {
      cart.value = cart.value.filter(item => item.id !== productId)
    }
  }

  const cartTotal = computed(() => {
    return cart.value.reduce((sum, item) => sum + (item.price * item.quantity), 0)
  })

  return { cart, addToCart, removeFromCart, cartTotal }
}