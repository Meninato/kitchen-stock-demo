'use client'

import { useEffect, useState } from 'react'

interface Props {
  texts?: string[]
  interval?: number // milliseconds
}

export const Loader = ({ texts = ['Preparando a sua cozinha...'], interval = 2000 }: Props) => {
  const [currentIndex, setCurrentIndex] = useState(0)
  const [fade, setFade] = useState(true)

  useEffect(() => {
    if (!texts || texts.length <= 1) return

    const id = setInterval(() => {
      setFade(false) // start fade out
      setTimeout(() => {
        setCurrentIndex((prev) => (prev + 1) % texts.length)
        setFade(true) // fade in new text
      }, 300) // fade duration
    }, interval)

    return () => clearInterval(id)
  }, [texts, interval])

  return (
    <div className="fixed inset-0 bg-white z-50 flex items-center justify-center">
      <div className="text-center">
        <div className="w-16 h-16 border-4 border-blue-600 border-t-transparent rounded-full animate-spin mx-auto" />
        {texts.length > 0 && (
          <p
            className={`mt-4 text-gray-600 transition-opacity duration-300 ${
              fade ? 'opacity-100' : 'opacity-0'
            }`}
          >
            {texts[currentIndex]}
          </p>
        )}
      </div>
    </div>
  )
}