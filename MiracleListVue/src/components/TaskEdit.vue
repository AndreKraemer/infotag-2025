<template>
 <div style="padding: 0">
  <div>
   <h4 id="taskheadline">Task #{{ task?.taskID }}</h4>
   <form id="taskForm">
    <!--Schaltflächen-->
    <button id="save" type="button" title="Änderungen speichern" @click="Save" class="btn btn-success" style="margin-right: 5px">
     <span class="glyphicon glyphicon-floppy-save"></span>
     <span class="hidden-xs">Save</span>
    </button>
    <button id="cancel" type="button" title="Änderungen verwerfen" @click="Cancel" class="btn btn-warning">
     <span class="glyphicon glyphicon-remove"></span>
     <span class="hidden-xs">Cancel</span>
    </button>
    <button 
      v-if="isHybridAvailable || isShareAPIAvailable" 
      id="share" 
      type="button" 
      title="Share this task" 
      @click="handleShareTask" 
      class="btn share-button">
      <span class="glyphicon glyphicon-share"></span>
      <span class="hidden-xs">Share</span>
    </button>    
    <!--Validierungsausgabe-->
    <div v-if="v$.$error" class="alert alert-danger">
     <h4 class="alert-heading">There are validation errors in this form:</h4>
     <li v-for="error of v$.$errors" :key="error.$uid">
      <span>{{ error.$property }}: {{ error.$validator }} ({{ error.$message }})</span>
     </li>
    </div>

    <!--Titel-->
    <div class="form-group" :class="{ 'has-error': v$.title.$error }">
     <label for="tasktitle" class="control-label">Title</label> <span style="color: red" v-if="v$.title.$error">*</span>
     <input id="tasktitle" name="tasktitle" type="text" v-model="t.title" required class="form-control" />
     <div class="text-danger" v-if="v$.title.$error">Required and at least 3 letters!</div>
    </div>
    <div class="row">
     <!--Wichtigkeit-->
     <div class="col-xs-3" style="padding-right: 2px">
      <div class="form-group">
       <label for="taskimportance" class="control-label">Importance</label>
       <select id="taskimportance" name="taskimportance" v-model="t.importance" class="form-control">
        <option
         v-for="x in Object.keys(Importance).filter((item) => {
          return isNaN(Number(item));
         })"
         :key="x"
         :value="Importance[x]">
         {{ x }}
        </option>
       </select>
      </div>
     </div>
     <!--Aufwand-->
     <div class="col-xs-3" style="padding-left: 2px; padding-right: 2px">
      <label for="taskeffort" class="control-label">Effort</label>
      <input id="taskeffort" type="number" name="taskeffort" v-model="t.effort" class="form-control" />
     </div>
     <!--Fälligkeit-->
     <div class="col-xs-6" style="padding-left: 2px">
      <div class="form-group" :class="{'has-error': v$.due.$error}">
       <label for="taskDue" class="control-label" :title="t.due?.toString()">Due</label> <span style="color: red" v-if="v$.due.$error">*</span>
       <input id="taskdue" name="taskdue" type="date" v-model="taskDue" class="form-control" />
      </div>
     </div>
    </div>
    <!--Ende row-->
    <!--Unteraufgaben-->
    <SubTaskList :task="task"></SubTaskList>
    <!--Notiz-->
    <div class="form-group">
     <label for="tasknote" class="control-label">Note</label>
     <textarea id="tasknote" name="tasknote " rows="5" v-model="t.note" class="form-control"></textarea>
    </div>
   </form>
  </div>
 </div>
</template>

<script setup lang="ts">
import { computed, watch, ref, onMounted } from 'vue';
import { Task, Importance } from '@/services/MiracleListProxyV2';
import SubTaskList from '@/components/SubTaskList.vue';
import { useHybridWebView } from "@/composables/useHybridWebView";
import { useToast } from "vue-toastification";

// Sprint 5
import useVuelidate from '@vuelidate/core';
import { required, minLength, helpers } from '@vuelidate/validators';
import moment from 'moment';


// Initialize hybrid web view composable
const { isAvailable: isHybridAvailable, shareTask } = useHybridWebView();
const toast = useToast();
const isSharing = ref(false);
const isShareAPIAvailable = ref(false);

// Check if Web Share API is available in this browser
onMounted(() => {
  isShareAPIAvailable.value = typeof navigator.share === 'function';
});


//#region Öffentliche Schnittstelle der Komponente
// === Parameter
const props = defineProps({
 task: Task
});
// ===  Events
const emit = defineEmits<{
    TaskEditDone: [changed: boolean]
}>();
//#endregion

// Wrapper für Parameter t, weil Zwei-Wege-Bindung direkt an Parameter nicht möglich
const t = computed(() => props.task);
// Alternative:
// let t = ref(props.task);
// watch(() => props.task, (newValue, oldValue) => {
//   t.value = newValue
// });

watch(
 () => props.task,
 () => {
  v$.value.$reset();
 }
);

// Wrapper für due -> Workaround für Datumsformatproblem
const taskDue = computed({
 get: () => (t.value!.due ? t.value!.due!.toISOString().substring(0, 10) : null),
 set: (event) => (t.value!.due = new Date(event as string))
});

//#region Sprint 5 Validation

// Custom Validator
const future = (value: Date) => value >= moment(new Date()).startOf('day').toDate();

// Regeln
const rules = {
 title: {
  required,
  minLength: helpers.withMessage('Title must be at least 3 chars long!', minLength(3))
 },
 due: {
  required,
  future: helpers.withMessage('Due date must be in the future!', future)
 }
};

const v$ = useVuelidate(rules, props.task as any, { $autoDirty: true });
//#endregion

//#region Benutzeraktionen
async function Save() {
 var ok = await v$.value.$validate();
 if (ok) {
  console.log("Save");
  emit("TaskEditDone", true);
 }
 else
 {
  console.warn("Validation failed!", v$.value.$errors);
 }
}

function Cancel() {
 console.log('Cancel');
 emit('TaskEditDone', false);
}

// Share task with native functionality
async function handleShareTask() {
  if (!t.value) return;
  
  isSharing.value = true;
  
  try {
    const result = await shareTask(t.value);
    
    if (result.success) {
      toast.success(result.message || "Task shared successfully!");
    } else {
      toast.error(result.message || "Failed to share task");
    }
  } catch (error) {
    console.error("Error sharing task:", error);
    toast.error("An error occurred while sharing");
  } finally {
    isSharing.value = false;
  }
}

//#endregion
</script>

<style scoped>
.share-button {
  margin-left: 5px;
  background-color: #007bff;
  color: white;
  border: #02468f;
  transition: background-color 0.2s ease;
}

.share-button:hover {
  background-color: #0056b3;
}
</style>
