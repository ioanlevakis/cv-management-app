import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import fs from 'fs'
import path from 'path'

const certificateDirectory = path.resolve(__dirname, 'src/certificates')
const certificatePath = path.resolve(certificateDirectory, 'certificate.crt')
const privateKeyPath = path.resolve(certificateDirectory, 'privateRSA.key')

export default defineConfig({
  plugins: [react()],

  server: {
    https: {
      cert: fs.readFileSync(certificatePath),
      key: fs.readFileSync(privateKeyPath),
    },
    host: 'localhost',
    port: 3000
  }
})