import path from 'path'
import svgr from 'vite-plugin-svgr'


export default {
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src')
    }
  },
  plugins: [svgr()]
}
