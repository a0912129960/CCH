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
  width: 80px;
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
}

button:hover {
  background: #fab710;
  border: 1px solid #fab710;
}
</style>
