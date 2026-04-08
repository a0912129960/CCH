<template>
  <div class="dropdown-box">
    <button
      type="button"
      class="dropdown-btn"
      :class="isClicked ? 'active-dropdown' : ''"
      @click="toggleDropdown"
      @blur="closeDropdown"
    >
      <span class="page-size-txt">{{ pageSize }}</span
      ><icon-arrow-dropdown />
    </button>
    <menu class="dropdown" v-if="isClicked">
      <li
        @mousedown="pageSizeHandler(5)"
        :class="pageSize == 5 ? 'active-item' : ''"
      >
        5
      </li>
      <li
        @mousedown="pageSizeHandler(10)"
        :class="pageSize == 10 ? 'active-item' : ''"
      >
        10
      </li>
      <li
        @mousedown="pageSizeHandler(15)"
        :class="pageSize == 15 ? 'active-item' : ''"
      >
        15
      </li>
    </menu>
  </div>
</template>

<script>
import IconArrowDropdown from "../icons/icon-arrow-dropdown.vue";

export default {
  components: {
    IconArrowDropdown,
  },
  data() {
    return {
      pageSize: 5,
      isClicked: false,
    };
  },
  methods: {
    toggleDropdown() {
      this.isClicked = !this.isClicked;
    },
    closeDropdown() {
      this.isClicked = false;
    },
    pageSizeHandler(value) {
      if (value === this.pageSize) return;
      this.pageSize = value;
      this.isClicked = !this.isClicked;
      this.$emit("page-size", this.pageSize);
    },
  },
};
</script>

<style scoped>
.dropdown-btn {
  width: 56px;
  height: 40px;
  background: #ffffff;
  border: 1px solid #e5e5e5;
  border-radius: 2px;
  cursor: pointer;
  margin-bottom: 1px;
}

.dropdown-btn:hover,
.dropdown-btn:active,
.active-dropdown {
  border: 1px solid #00a8e2;
}

.page-size-txt {
  margin-right: 10px;
}

.dropdown {
  box-shadow: 0px 4px 4px rgba(0, 0, 0, 0.25);
  border-radius: 0px 0px 4px 4px;
}

.dropdown li {
  list-style: none;
  width: 56px;
  height: 45px;
  text-align: center;
  line-height: 3;
  cursor: pointer;
}

.dropdown li:hover,
.dropdown li:active,
.active-item {
  background-color: #e7eef1;
}
</style>
