<template>
  <div class="progress-bar">
    <div class="progress-line" :style="{ width: progress + '%' }"></div>
    <svg
      class="svg-dot"
        width="4"
        height="4"
        viewBox="0 0 4 4"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
      <circle cx="2" cy="2" r="2" fill="#00A8E2" />
    </svg>
  </div>
</template>

<script>
export default {
  props: ["progress"],
};
</script>

<style scoped>
.progress-bar {
  height: 1px;
  background-color: #d9d9d9;
  display: flex;
}

.progress-line {
  height: 1px;
  background-color: #00a8e2;
}

.svg-dot {
  transform: translate(-50%, -40%);
}
</style>
