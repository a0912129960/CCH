<template>
    <button @click="callToggle" class="toggle_btn">
      <svg v-if="!collapsed" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="toggle feather feather-chevron-right">
        <polyline points="9 18 15 12 9 6"></polyline>
      </svg>
      <svg v-if="collapsed" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="toggle feather feather-chevron-down">
        <polyline points="6 9 12 15 18 9"></polyline>
      </svg>
    </button>
</template>
  
<script>
export default {
  data(){
    return {
      collapsed: true
    }
  },
  methods:{
    callToggle(){
      this.onToggle();
      this.collapsed = !this.collapsed;
    }
  },
  props:{
    onToggle: {
      type: Function,
      required: true
    }
  }
};
</script>
  
<style>
button {
  background: none;
  border: none;
  cursor: pointer;
  outline: none;
}
.toggle_btn{
  width:40px
}
.toggle{
  color:#00a8e2;
}
</style>
  