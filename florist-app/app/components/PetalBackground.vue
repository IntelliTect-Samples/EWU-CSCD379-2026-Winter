<template>
  <div class="petal-container">
    <div 
      v-for="n in 100" 
      :key="n" 
      :class="['petal', n % 2 === 0 ? 'shape-rose' : 'shape-rose2']"
      :style="generatePetalStyle()"
    ></div>
  </div>
</template>

<script setup>
import '~/assets/css/PetalBackground.css'

const generatePetalStyle = () => {
  const left = Math.random() * 100;
  const delay = Math.random() * 20;
  const duration = 20 + Math.random() * 15;
  const size = 10 + Math.random() * 15;
  const sway = (Math.random() - 0.5) * 400; 

  return {
    left: `${left}%`,
    animationDelay: `${-delay}s`,
    animationDuration: `${duration}s`,
    width: `${size}px`,
    height: `${size}px`,
    '--sway-amount': `${sway}px`,
    opacity: 0.3 + Math.random() * 0.5,
    backgroundColor: Math.random() > 0.5 ? '#f8bbd0' : '#ffdae9'
  };
};
</script>

