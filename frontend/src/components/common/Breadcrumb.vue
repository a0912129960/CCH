<template>
    <el-breadcrumb :separator-icon="arrowRight">
        <template v-for="(item, index) in breadcrumbList" :key="index">
            <el-breadcrumb-item :to="{ path: item.path }">{{item.name}}</el-breadcrumb-item>
        </template>
    </el-breadcrumb>
    
</template>

<script>
import { ArrowRight } from '@element-plus/icons-vue';
export default {
    data() {
        return {
            arrowRight: ArrowRight
        };
    },
    props: {
        breadcrumbList: Array
    }
};
</script>

<style>
</style>
