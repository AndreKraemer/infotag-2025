import { computed, ref } from "vue";
import { LoginInfo } from "./MiracleListProxyV2";

/**
 * Globaler Anwendungszustand mit statischen Mitgliedern
 * Alternative Flux via Vuex4 für Vue.js 3.x https://next.vuex.vuejs.org/ oder Pinia https://pinia.vuejs.org/
 */
export class AppState {

 // Reactive Properties
 public static CurrentLoginInfo = ref<LoginInfo | null>();
 public static Username = computed(()=> AppState.CurrentLoginInfo.value?.username ?? "");
 
 // For MAUI demo - track current category
 public static CurrentCategoryID = ref<number | null>(null);

 // Normale Properties
 public static get Token(): string {
  return AppState.CurrentLoginInfo.value?.token ?? "";
 }
 public static get Authenticated(): boolean {
  return AppState.Token != "";
 }

 public static HubConnectionInfo = ref("");
}