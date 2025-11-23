import * as React from "react"
import { motion, AnimatePresence } from "framer-motion"

type Placement =
    | "north"
    | "north-east"
    | "east"
    | "south"
    | "south-east"
    | "west"
    | "north-west"
    | "south-west"

type Props = {
    message: React.ReactNode
    children: React.ReactNode
    placement?: Placement
}

export default function Tooltip({ message, children, placement = "north" }: Props) {
    const [visible, setVisible] = React.useState(false)

    // determine container class and tail class based on placement
    let containerClass = "left-1/2 -translate-x-1/2 bottom-full mb-3"
    let tailClass = "absolute left-1/2 -translate-x-1/2 -bottom-1 w-3 h-3 bg-gray-900 rotate-45"

    switch (placement) {

        case "east":
            // center the bubble vertically within the wrapper
            containerClass = "top-0 bottom-0 flex items-center left-full ml-3"
            tailClass = "absolute -left-1 top-1/2 -translate-y-1/2 w-3 h-3 bg-gray-900 rotate-45"
            break
        case "south":
            containerClass = "left-0 right-0 flex justify-center top-full mt-3"
            tailClass = "absolute left-1/2 -translate-x-1/2 -top-1 w-3 h-3 bg-gray-900 rotate-45"
            break
        case "west":
            // center the bubble vertically within the wrapper
            containerClass = "top-0 bottom-0 flex items-center right-full mr-3"
            tailClass = "absolute -right-1 top-1/2 -translate-y-1/2 w-3 h-3 bg-gray-900 rotate-45"
            break
        case "north":
        default:
            // center the bubble horizontally within the wrapper
            containerClass = "left-0 right-0 flex justify-center bottom-full mb-3"
            tailClass = "absolute left-1/2 -translate-x-1/2 -bottom-1 w-3 h-3 bg-gray-900 rotate-45"
            break
    }

    return (
        <span
            className="relative inline-block"
            tabIndex={0}
            onMouseEnter={() => setVisible(true)}
            onMouseLeave={() => setVisible(false)}
            onFocus={() => setVisible(true)}
            onBlur={() => setVisible(false)}
        >
            <span className="inline-block">{children}</span>

            <AnimatePresence>
                {visible && message ? (
                    <motion.div
                        initial={{ opacity: 0, y: 6, scale: 0.96 }}
                        animate={{ opacity: 1, y: 0, scale: 1 }}
                        exit={{ opacity: 0, y: 6, scale: 0.96 }}
                        transition={{ duration: 0.14 }}
                        className={`pointer-events-none absolute z-50 ${containerClass}`}
                    >
                        <div className="relative">
                            <div className="bg-gray-900 text-white text-sm px-3 py-2 shadow min-w-[200px] max-w-xs" style={{ borderRadius: 8 }}>
                                {message}
                            </div>

                            <div className={tailClass} aria-hidden />
                        </div>
                    </motion.div>
                ) : null}
            </AnimatePresence>
        </span>
    )
}
