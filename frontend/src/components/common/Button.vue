<template>
  <button :class="mode" :label="label" :disabled="disabled">
    <slot>{{ label }}</slot>
  </button>
</template>

<script>
export default {
  props: ["mode", "label", "disabled"],
};
</script>

<style scoped>
button {
  min-width: 80px;
  padding: 0 16px;
  height: 40px;
  border: 1px solid #f68b39;
  background: linear-gradient(180deg, #f68b39 0%, #ff6b00 100%);
  box-shadow: 0px 2px 2px rgba(0, 0, 0, 0.15);
  border-radius: 0px 0px 15px 0px;
  font-family: MyDimerco-WorkSansBold;
  font-style: normal;
  font-size: 14px;
  color: #fff;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s ease;
}

button:hover {
  background: #fab710;
  border: 1px solid #fab710;
}

/* Outline Mode (外框模式) */
button.btn-outline {
  background: #fff;
  color: #f68b39;
  border: 1px solid #f68b39;
  box-shadow: none;
}

button.btn-outline:hover {
  background: #fff5ed;
  border-color: #ff6b00;
  color: #ff6b00;
}
</style>
