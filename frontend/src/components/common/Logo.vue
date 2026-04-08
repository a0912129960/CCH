<template>
  <component :is="iconType"></component>
</template>

<script>
import IconPlane from "../icons/icon-large-plane.vue";
import IconShip from "../icons/icon-large-ship.vue";
import IconTruck from "../icons/icon-large-truck.vue";
import IconRail from "../icons/icon-large-rail.vue";
import IconWarehouse from "../icons/icon-large-warehouse.vue";
import IconOutbound from '../icons/icon-outbound-trackntrace.vue'
import IconInbound from '../icons/icon-inbound-trackntrace.vue'
import IconReady from '../icons/icon-readytogo.vue'

export default {
  props: ['icon-type'],
  data() {
    return {
      iconPlane: "icon-plane",
      iconShip: "icon-ship",
      iconTruck: "icon-truck",
      iconRail: "icon-rail",
      iconWarehouse: "icon-warehouse",
      iconOutbound: "icon-outbound",
      iconInbound: "icon-inbound",
      iconReady: "icon-ready"
    };
  },
  components: { 
    IconPlane, 
    IconShip, 
    IconTruck, 
    IconRail, 
    IconOutbound,
    IconInbound,
    IconWarehouse,
    IconReady
  },
};
</script>
