<template>
    <div class="search-input__container">
        <input
            :placeholder="$t('ShipmentList.t_t.search')"
            type="text"
            v-model="searchInput"
            @keypress.enter.prevent="getTrackingCode"
        />
        <p class="search-input__container--err-msg" v-if="isError">
            Tracking number requires at least 3 characters
        </p>
    </div>
</template>

<script>
export default {
    props: ["input-code"],
    data() {
        return {
            searchInput: "",
            isError: false,
        };
    },
    methods: {
        getTrackingCode() {
            if (!this.validateSearch) {
                this.isError = true;
                return;
            }
            this.$emit("get-tracking-code", this.searchInput);
            this.isError = false;
        },
    },
    computed: {
        validateSearch() {
            return this.searchInput.length < 3 ? false : true;
        },
    },
};
</script>

<style lang="scss" scoped>
.search-input__container {
    input {
        width: 450px;
        height: 40px;
        padding-left: 40px;
        border: 1px solid #ced4da;
        border-radius: 4px;
        background: border-box #e7eef1
            url("../../assets/images/icons/search-icon.svg") no-repeat 3% 50%;
        font-family: MyDimerco-OpenSansRegular;
        font-size: 14px;
        color: #8b95a0;
        &:focus {
            background: border-box #fff
                url("../../assets/images/icons/search-icon.svg") no-repeat 3%
                50%;
            outline: none;
        }
    }
    &--err-msg {
        color: #ff0033;
        position: absolute;
        top: 75px;
        font-size: 12px;
        font-family: MyDimerco-OpenSansRegular;
    }
}
</style>
