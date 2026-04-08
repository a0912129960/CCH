<template>
	<div class="col-12">
		<div class="dmc__info-row">
			<p class="dmc__info-row--label">{{ label }}</p>
			<div class="dmc__cell">
				<div class="dmc__info-row--content row" >{{ content }}</div>
				<div v-if="isCopy && content" @click="copy(content)" class="tooltip__layout">
					<img src="../../assets/images/copy-outlined.svg" alt="" srcset="" class="icon" />
					<span class="tooltip__content" :class="{'show': flag}">Copied!</span>
				</div>

			</div>
		</div>
	</div>
</template>

<script>
import { copyToClipboard } from '../../utils';

export default {
	props: {
		isCopy: Boolean,
		label: String,
		content: [String, Number],
	},
	data: function () {
		return {
			flag: false
		}
	},
	methods: {
		copy(value) {
			copyToClipboard(value);
			this.flag = true
			setTimeout(() => {
				this.flag = false
			}, 1000)
		}
	}
};
</script>

<style lang="scss">
.dmc__cell {
	display: flex;
}
.tooltip__layout {
	top: 2px;
	position: relative;
	display: inline-block;
}

.tooltip__content.show {
	opacity: 1;
	transition: 1s;
	-webkit-transition: 0.2s;
	transition: 0.2s;
}

.tooltip__content {
	padding-left: 50px;
	right: -65px;
	top: -10px;
	background: #616161;
	border-radius: 2px;
	-webkit-box-shadow: 0 3px 1px -2px rgba(0, 0, 0, .2), 0 2px 2px 0 rgba(0, 0, 0, .14), 0 1px 5px 0 rgba(0, 0, 0, .12);
	box-shadow: 0 3px 1px -2px rgba(0, 0, 0, .2), 0 2px 2px 0 rgba(0, 0, 0, .14), 0 1px 5px 0 rgba(0, 0, 0, .12);
	color: #fff;
	display: inline-block;
	font-size: 12px;
	padding: 5px 8px;
	position: absolute;
	-webkit-transition: 0.2s;
	transition: 0.2s;
	width: auto;
	opacity: 0;
}

.dmc__info-row {
	&--label {
		color: #8b95a0;
		font-size: 14px;
		font-weight: 600;
		padding-bottom: 15px;
	}

	&--content {
	font-family: MyDimerco-OpenSansBold;
	color: #465363;
	font-size: 16px;
	padding-right: 10px;
	white-space: pre-line;
	}

	.icon {
		cursor: pointer;
	}
}
</style>
