<template>
    <div class="dmc__alert" :class="`dmc__alert-${open ? 'open' : 'hide'} dmc__alert-${type}`">
        <p class="dmc__alert-close">
            <el-icon @click="() => hideMessage(0)">
                <Close />
            </el-icon>
        </p>
        <h3>{{ title }}</h3>
        <p>{{ text }}</p>

    </div>
</template>
<script>
import { useAppStore } from "@/stores";
const appStore = useAppStore();
export default {
    props: {
        open: Boolean,
        type: String,
        title: String,
        text: String
    },
    methods: {
        hideMessage(timeout) {
            setTimeout(function () {
                appStore.setMessage({
                    open: false,
                    status: "",
                    title: "",
                    text: "",
                })
            }, timeout);
        }
    },
    mounted() {
        this.hideMessage(4000);
    },
}
</script>
<style scoped lang="scss">
@use '@/scss/variables' as *;

.dmc__alert {
    position: fixed;
    top: 5rem;
    right: 0rem;
    width: 330px;
    overflow: hidden;
    box-shadow: rgba(99, 99, 99, 0.2) 0px 2px 8px 0px;
    padding: 0.8rem 1rem;
    transition: 0.3s ease-in-out;
    display: none;
    z-index: 3001;
    font-family: MyDimerco-OpenSansRegular;
    background-color: $white;
    color: $black;
    border-radius: 5px;

    h3{
        margin-bottom: 5px;
    }

    p{
        font-size: 13px;
    }

    &-open {
        right: 2rem;
        display: block;
    }
    &-hide {
        right: 0rem;
        display: none;
    }

    &-success {
        box-shadow: rgba(21, 180, 106, 0.2) 0px 2px 8px 0px;
        
        border-left: 5px solid $success-color;
    }

    &-warning {
        box-shadow: rgba(224, 142, 34, 0.2) 0px 2px 8px 0px;
        border-left: 5px solid $warning-color;
    }

    &-error {
        box-shadow: rgba($danger-color, 0.4) 0px 2px 8px 0px;
        border-left: 5px solid $danger-color;
    }

    &-close {
        text-align: right;
        cursor: pointer;
        position: absolute;
        right: 20px;
    }
}
</style>