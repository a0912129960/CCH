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
  border: 1px solid #d9d9d9;
  border-radius: 4px;
}

/* .basecard {
  height: 194px;
} */

.grid-basecard {
  display: grid;
  grid-template-columns: .05fr 0.15fr 0.98fr .05fr;
  padding: 14px 0;
  /* width: max-content; */
  /* min-width: 100%;
  height: unset;
  min-height: 194px; */
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
