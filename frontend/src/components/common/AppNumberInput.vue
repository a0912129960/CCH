<template>
  <el-input
    :model-value="input"
    @update:modelValue="inputValue => input = formatValue(inputValue)"
    @blur="handleBlur"
  />
</template>


<script>
export default {
  props: {
    modelValue: {
      type: Number,
      default: null
    },
    precision: {
      type: Number,
      default: 0
    }
  },
  emits: ['update:modelValue'],
  data() {
    return {
      input: ''
    }
  },
  watch: {
    modelValue: {
      immediate: true,
      handler(value) {
        if (!value) return
        this.input = this.handlePrecision(value)
      }
    },
  },
  methods: {
    formatValue(inputValue) {
      if (!inputValue) return ''

      const includeDot = inputValue.includes('.')
      inputValue = this.filterNotNumber(inputValue)
      const[integer, decimal] =inputValue.split('.')

          const display = Number(integer).toLocaleString('en-US')
      return includeDot ? display + '.' + decimal : display
    },
    handleBlur() {
      const value = this.filterNotNumber(this.input)      
      this.$emit('update:modelValue', value ? Number(value) : null)
      return this.handlePrecision(value)
    },
    handlePrecision(inputValue) {
        return Number(inputValue).toLocaleString('en-US', {
        minimumFractionDigits: this.precision,
        maximumFractionDigits: this.precision
      });
    },
    filterNotNumber(inputValue) {
      return inputValue.replace(/[^\d.]/g, '');
    },
  }
}
</script>

<style scoped lang="scss"></style>
