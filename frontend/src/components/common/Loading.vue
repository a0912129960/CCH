<template>
	<div class="loading">
		<img src="@/assets/images/spin-logo.gif" alt="" srcset="" />
	</div>
</template>

<script>
export default {};
</script>

<style lang="scss">
.loading {
    position: fixed;
    top: 0;
    left: 0;
    background-color: rgba(0, 0, 0, 0.2);
    width: 100%;
    min-height: 100vh;
    display: flex;
    align-items: center;
    justify-content: center;
    z-index: 3002;
}
</style>
