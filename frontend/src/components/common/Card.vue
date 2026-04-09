<template>
  <div :class="layout">
    <slot></slot>
  </div>
</template>

<script>
export default {
  props: ["layout"],
};
</script>

<style scoped>
div {
  background-color: #ffffff;
  border: 1px solid rgba(0, 0, 0, 0.05);
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

div:hover {
  box-shadow: 0 6px 16px rgba(0, 0, 0, 0.08);
}

.grid-basecard {
  display: grid;
  grid-template-columns: .05fr 0.15fr 0.98fr .05fr;
  padding: 14px 0;
}

@media screen and (max-width: 1440px) {
  /* .basecard {
    height: 176px;
  } */
  .grid-basecard {
    min-height: 176px;
    height: unset;
  }
}
</style>
