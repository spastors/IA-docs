$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$outputDirectory = Join-Path $PSScriptRoot '..\assets'
[void][System.IO.Directory]::CreateDirectory($outputDirectory)
$outputPath = Join-Path $outputDirectory 'neurona-artificial.png'
$bitmap = [System.Drawing.Bitmap]::new(3400, 2360)
$g = [System.Drawing.Graphics]::FromImage($bitmap)
$g.ScaleTransform(2, 2)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$g.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::AntiAliasGridFit
$g.Clear([System.Drawing.ColorTranslator]::FromHtml('#F5F7FB'))

function Label($Text, $X, $Y, $Size = 24, $Color = '#152A45', $Bold = $false, $Centered = $false) {
    $style = if ($Bold) { [System.Drawing.FontStyle]::Bold } else { [System.Drawing.FontStyle]::Regular }
    $font = [System.Drawing.Font]::new('Segoe UI', $Size, $style, [System.Drawing.GraphicsUnit]::Pixel)
    $brush = [System.Drawing.SolidBrush]::new([System.Drawing.ColorTranslator]::FromHtml($Color))
    if ($Centered) { $X -= $g.MeasureString($Text, $font).Width / 2 }
    $g.DrawString($Text, $font, $brush, [float]$X, [float]$Y)
    $font.Dispose()
    $brush.Dispose()
}

function Box($X, $Y, $W, $H, $Fill = '#FFFFFF', $Stroke = '#DCE3EE', $Radius = 18) {
    $path = [System.Drawing.Drawing2D.GraphicsPath]::new()
    $d = $Radius * 2
    $path.AddArc($X, $Y, $d, $d, 180, 90)
    $path.AddArc(($X + $W - $d), $Y, $d, $d, 270, 90)
    $path.AddArc(($X + $W - $d), ($Y + $H - $d), $d, $d, 0, 90)
    $path.AddArc($X, ($Y + $H - $d), $d, $d, 90, 90)
    $path.CloseFigure()
    $brush = [System.Drawing.SolidBrush]::new([System.Drawing.ColorTranslator]::FromHtml($Fill))
    $pen = [System.Drawing.Pen]::new([System.Drawing.ColorTranslator]::FromHtml($Stroke), 2)
    $g.FillPath($brush, $path)
    $g.DrawPath($pen, $path)
    $brush.Dispose(); $pen.Dispose(); $path.Dispose()
}

function Circle($X, $Y, $R, $Fill, $Stroke) {
    $brush = [System.Drawing.SolidBrush]::new([System.Drawing.ColorTranslator]::FromHtml($Fill))
    $pen = [System.Drawing.Pen]::new([System.Drawing.ColorTranslator]::FromHtml($Stroke), 2.5)
    $g.FillEllipse($brush, ($X - $R), ($Y - $R), (2 * $R), (2 * $R))
    $g.DrawEllipse($pen, ($X - $R), ($Y - $R), (2 * $R), (2 * $R))
    $brush.Dispose(); $pen.Dispose()
}

function Arrow($X1, $Y1, $X2, $Y2, $Color = '#61728B') {
    $pen = [System.Drawing.Pen]::new([System.Drawing.ColorTranslator]::FromHtml($Color), 3)
    $cap = [System.Drawing.Drawing2D.AdjustableArrowCap]::new(5, 6)
    $pen.CustomEndCap = $cap
    $g.DrawLine($pen, [float]$X1, [float]$Y1, [float]$X2, [float]$Y2)
    $pen.Dispose(); $cap.Dispose()
}

Label 'Neurona artificial: calcular y aprender' 65 40 46 '#152A45' $true
Label 'Las entradas se ponderan, se combinan con un sesgo y pasan por una activación.' 68 107 25 '#61728B'

Label '01  CÁLCULO DE UNA NEURONA' 70 163 22 '#245FC5' $true
Box 60 205 1580 345
Label 'Entradas' 170 222 23 '#24654B' $true $true
Label 'Pesos' 400 222 23 '#245FC5' $true $true
Label 'Sesgo' 700 222 23 '#8E6511' $true $true
Label 'Activación' 1010 222 23 '#6951A1' $true $true
Label 'Salida' 1400 222 23 '#AA4D39' $true $true

Circle 170 315 40 '#E5F4EA' '#6AA784'
Circle 170 445 40 '#E5F4EA' '#6AA784'
Label 'x₁' 170 289 37 '#24654B' $true $true
Label 'x₂' 170 419 37 '#24654B' $true $true
Arrow 215 315 331 315
Arrow 215 445 331 445
Box 340 283 120 64 '#E9F0FF' '#A0B8E5'
Box 340 413 120 64 '#E9F0FF' '#A0B8E5'
Label '× w₁' 400 294 30 '#245FC5' $true $true
Label '× w₂' 400 424 30 '#245FC5' $true $true
Arrow 468 315 642 356
Arrow 468 445 642 404
Circle 700 380 62 '#E9F0FF' '#7099D9'
Label 'Σ' 700 334 66 '#245FC5' $false $true
Box 660 257 80 44 '#FFF4D9' '#E0C879' 12
Label 'b' 700 257 32 '#8E6511' $true $true
Arrow 700 305 700 315
Arrow 770 380 895 380
Label 'z' 832 334 27 '#61728B' $false $true
Box 905 335 210 90 '#F0EAFA' '#B7A2D3'
Label 'f(z)' 1010 349 40 '#6951A1' $true $true
Arrow 1125 380 1338 380
Circle 1400 380 52 '#FDEDE6' '#D8A18C'
Label 'ŷ' 1400 339 56 '#AA4D39' $true $true
Label 'z = w₁·x₁ + w₂·x₂ + b' 700 471 27 '#152A45' $false $true
Label 'ŷ = f(z)' 1400 471 27 '#152A45' $false $true

Label '02  EJEMPLO: APRENDER A SUMAR' 70 585 22 '#245FC5' $true
Box 60 627 470 225
Label 'Datos de entrenamiento' 88 649 27 '#152A45' $true
Label 'x₁       x₂       objetivo y' 95 697 25 '#61728B'
Label '1         4              5' 98 738 26
Label '3         6              9' 98 771 26
Label '6         2              8' 98 804 26
Arrow 546 741 600 741
Box 620 627 470 225 '#EEF5FF' '#C6D7EF'
Label 'Tras ajustar los parámetros' 648 649 27 '#152A45' $true
Label 'w₁ ≈ 1      w₂ ≈ 1      b ≈ 0' 648 704 27 '#245FC5' $true
Label 'Activación lineal: f(z) = z' 648 751 26
Label 'Regla aprendida: ŷ ≈ x₁ + x₂' 648 794 26
Arrow 1104 741 1158 741
Box 1178 627 462 225 '#EAF6EE' '#B8DCC4'
Label 'Predicción con datos nuevos' 1203 649 26 '#24654B' $true
Label 'x₁ = 4      x₂ = 7' 1409 711 30 '#24654B' $false $true
Label 'ŷ ≈ 4 + 7 = 11' 1409 767 35 '#24654B' $true $true

Label '03  ENTRENAMIENTO: REPETIR Y REDUCIR LA PÉRDIDA' 70 887 22 '#245FC5' $true
Box 60 934 345 110
Label '1. Predecir ŷ' 232 965 27 '#152A45' $true $true
Arrow 419 989 459 989
Box 475 934 345 110
Label '2. Comparar con y' 647 965 27 '#152A45' $true $true
Arrow 834 989 874 989
Box 890 934 345 110
Label '3. Ajustar pesos y sesgo' 1062 970 24 '#152A45' $true $true
Arrow 1249 989 1289 989
Box 1305 934 335 110
Label '4. Repetir' 1472 965 27 '#152A45' $true $true
Label 'La pérdida mide el desacierto; el optimizador guía los ajustes de w y b.' 70 1080 25 '#61728B'
Label 'Neurona moderna simplificada · La suma solo necesita una salida lineal.' 70 1125 22 '#61728B'

$bitmap.Save($outputPath, [System.Drawing.Imaging.ImageFormat]::Png)
$g.Dispose()
$bitmap.Dispose()
Write-Output $outputPath
