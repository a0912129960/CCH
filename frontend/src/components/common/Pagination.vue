<template>
	<el-pagination
		:page-size="pageSize"
		:pager-count="11"
		layout="prev, pager, next"
		:total="total"
		@current-change="handleCurrentChange"
		:page-count="pageCount"
	>
	</el-pagination>
</template>

<script scoped>
export default {
	props: ["total", "handleCurrentChange", "pageCount", "pageSize"],
	data() {},
};
</script>

<style>
.el-pager li {
	border-left: 0;
	font-weight: bold;
	margin: 0 3px;
}

li.is-active.number,
.el-pager li:hover {
	color: #011837;
	border: 1px solid #b7b8bb;
	border-left: 1px solid #b7b8bb !important;
}
</style>
