<script setup lang="ts">
import InputText from 'primevue/inputtext'

defineProps<{
    name: string
    label: string
    placeholder?: string
    modelValue: string | null
}>()

const emit = defineEmits<{
    (e: 'update:modelValue', modelValue: string | null): void
}>()

function handleInput(e: Event) {
    emit('update:modelValue', (e.target as HTMLInputElement).value)
}
</script>

<template>
    <div class="field col">
        <label :for="name">{{ label }}</label>
        <InputText
            type="text"
            :id="name"
            :name="name"
            :placeholder="placeholder"
            :value="modelValue"
            @input="handleInput"
            class="text-base text-color surface-overlay p-2 border-1 border-solid surface-border border-round appearance-none outline-none focus:border-primary w-full"
        />
    </div>
</template>
