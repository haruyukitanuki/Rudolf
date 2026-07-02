import path from 'node:path';
import { defineConfig } from 'vite';
import dts from 'vite-plugin-dts';

export default defineConfig({
  build: {
    lib: {
      entry: path.resolve(__dirname, 'src/index.ts'),
      formats: ['es', 'cjs'],
      fileName: (format) => (format === 'es' ? 'index.js' : 'index.cjs')
    },
    sourcemap: true,
    minify: false,
    rollupOptions: {
      external: []
    }
  },
  plugins: [
    dts({
      rollupTypes: true,
      include: ['src/**/*.ts']
    })
  ]
});
