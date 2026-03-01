import { useEffect, useMemo, useState } from "react";

type TextTypeProps = {
  text?: string[];
  texts?: string[];
  typingSpeed?: number;
  deletingSpeed?: number;
  pauseDuration?: number;
  showCursor?: boolean;
  cursorCharacter?: string;
  variableSpeedEnabled?: boolean;
  variableSpeedMin?: number;
  variableSpeedMax?: number;
  cursorBlinkDuration?: number;
  className?: string;
};

function getRandomSpeed(min: number, max: number): number {
  return Math.floor(Math.random() * (max - min + 1)) + min;
}

export default function TextType({
  text,
  texts,
  typingSpeed = 30,
  deletingSpeed = 50,
  pauseDuration = 1500,
  showCursor = true,
  cursorCharacter = "_",
  variableSpeedEnabled = false,
  variableSpeedMin = 60,
  variableSpeedMax = 120,
  cursorBlinkDuration = 0.5,
  className,
}: TextTypeProps) {
  const sourceTexts = useMemo(() => {
    const merged = texts?.length ? texts : text;
    return merged && merged.length > 0 ? merged : [""];
  }, [text, texts]);

  const [textIndex, setTextIndex] = useState(0);
  const [charIndex, setCharIndex] = useState(0);
  const [isDeleting, setIsDeleting] = useState(false);
  const [cursorVisible, setCursorVisible] = useState(true);

  const currentText = sourceTexts[textIndex] ?? "";
  const visibleText = currentText.slice(0, charIndex);

  useEffect(() => {
    const blinkMs = Math.max(100, Math.floor(cursorBlinkDuration * 1000));
    const id = window.setInterval(() => {
      setCursorVisible((prev) => !prev);
    }, blinkMs);

    return () => window.clearInterval(id);
  }, [cursorBlinkDuration]);

  useEffect(() => {
    let delay = isDeleting ? deletingSpeed : typingSpeed;

    if (variableSpeedEnabled && !isDeleting) {
      delay = getRandomSpeed(variableSpeedMin, variableSpeedMax);
    }

    if (!isDeleting && charIndex === currentText.length) {
      delay = pauseDuration;
    }

    const id = window.setTimeout(() => {
      if (!isDeleting) {
        if (charIndex < currentText.length) {
          setCharIndex((prev) => prev + 1);
          return;
        }
        setIsDeleting(true);
        return;
      }

      if (charIndex > 0) {
        setCharIndex((prev) => prev - 1);
        return;
      }

      setIsDeleting(false);
      setTextIndex((prev) => (prev + 1) % sourceTexts.length);
    }, delay);

    return () => window.clearTimeout(id);
  }, [
    charIndex,
    currentText.length,
    deletingSpeed,
    isDeleting,
    pauseDuration,
    sourceTexts.length,
    typingSpeed,
    variableSpeedEnabled,
    variableSpeedMax,
    variableSpeedMin,
  ]);

  return (
    <span className={className}>
      {visibleText}
      {showCursor ? (
        <span className={cursorVisible ? "opacity-100" : "opacity-0"}>{cursorCharacter}</span>
      ) : null}
    </span>
  );
}
