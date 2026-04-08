<template>
  <component :is="iconId"></component>
</template>

<script>
import IconHome from "../icons/icon-home.vue";
import IconLoad from "../icons/icon-load.vue";
import IconPackage from "../icons/icon-package.vue";
import IconPlaneFly from "../icons/icon-plane-fly.vue";
import IconPlaneLand from "../icons/icon-plane-land.vue";
import IconShip from "../icons/icon-ship.vue";
import IconTruck from "../icons/icon-truck.vue";
import IconRail from "../icons/icon-large-rail.vue";
import IconPiece from "../icons/icon-piece.vue";
import IconBook from "../icons/icon-book.vue";
import IconDone from "../icons/icon-done.vue";
import IconSpTruck from "../icons/icon-sp-truck.vue";
import IconReady from '../icons/icon-readytogo.vue'
import IconPacked from '../icons/icon-packed.vue'

export default {
  components: {
    IconHome,
    IconPackage,
    IconLoad,
    IconPlaneFly,
    IconPlaneLand,
    IconShip,
    IconTruck,
    IconRail,
    IconPiece,
    IconBook,
    IconDone,
    IconSpTruck,
    IconReady,
    IconPacked
  },
  props: ["iconId"],
  data() {
    return {
      isIcon: "icon-truck",
    };
  },
};
</script>

<style scoped>
svg {
  width: 29px;
  height: 30px;
}
</style>
