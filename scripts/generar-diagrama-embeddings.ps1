$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing

$outputPath = Join-Path $PSScriptRoot '..\assets\embeddings-coordenadas.png'
$bitmap = [System.Drawing.Bitmap]::new(3200, 2300)
$g = [System.Drawing.Graphics]::FromImage($bitmap)
$g.ScaleTransform(2, 2)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$g.TextRenderingHint = [System.Drawing.Text.TextRenderingHint]::AntiAliasGridFit
$g.Clear([System.Drawing.ColorTranslator]::FromHtml('#F5F7FB'))

function Text($Value, $X, $Y, $Size = 22, $Color = '#172B49', $Bold = $false) {
    $style = if ($Bold) { [System.Drawing.FontStyle]::Bold } else { [System.Drawing.FontStyle]::Regular }
    $font = [System.Drawing.Font]::new('Segoe UI', $Size, $style, [System.Drawing.GraphicsUnit]::Pixel)
    $brush = [System.Drawing.SolidBrush]::new([System.Drawing.ColorTranslator]::FromHtml($Color))
    $g.DrawString($Value, $font, $brush, [single]$X, [single]$Y)
    $font.Dispose()
    $brush.Dispose()
}

function Box($X, $Y, $W, $H, $Color = '#FFFFFF') {
    $brush = [System.Drawing.SolidBrush]::new([System.Drawing.ColorTranslator]::FromHtml($Color))
    $pen = [System.Drawing.Pen]::new([System.Drawing.ColorTranslator]::FromHtml('#D9E1ED'), 1.5)
    $g.FillRectangle($brush, $X, $Y, $W, $H)
    $g.DrawRectangle($pen, $X, $Y, $W, $H)
    $brush.Dispose()
    $pen.Dispose()
}

function Line($A, $B, $Color = '#DDE5EE', $Width = 1.4, $Dashed = $false, $Arrow = $false) {
    $pen = [System.Drawing.Pen]::new([System.Drawing.ColorTranslator]::FromHtml($Color), $Width)
    if ($Dashed) { $pen.DashStyle = [System.Drawing.Drawing2D.DashStyle]::Dash }
    if ($Arrow) { $pen.EndCap = [System.Drawing.Drawing2D.LineCap]::ArrowAnchor }
    $g.DrawLine($pen, [single]$A[0], [single]$A[1], [single]$B[0], [single]$B[1])
    $pen.Dispose()
}

function Point($At, $Color, $Radius = 9) {
    $brush = [System.Drawing.SolidBrush]::new([System.Drawing.ColorTranslator]::FromHtml($Color))
    $pen = [System.Drawing.Pen]::new([System.Drawing.Color]::White, 2)
    $g.FillEllipse($brush, ($At[0] - $Radius), ($At[1] - $Radius), (2 * $Radius), (2 * $Radius))
    $g.DrawEllipse($pen, ($At[0] - $Radius), ($At[1] - $Radius), (2 * $Radius), (2 * $Radius))
    $brush.Dispose()
    $pen.Dispose()
}

# Isometric projection: all three unit axes have equal projected length.
function Project([double]$X, [double]$Y, [double]$Z) {
    return @((540 + 180 * [Math]::Sqrt(3) * ($X - $Y)), (620 + 180 * ($X + $Y) - 360 * $Z))
}

function Distance($A, $B) {
    return [Math]::Sqrt([Math]::Pow($A[0] - $B[0], 2) + [Math]::Pow($A[1] - $B[1], 2) + [Math]::Pow($A[2] - $B[2], 2))
}

function Cosine($A, $B) {
    $dot = 0.0
    $normA = 0.0
    $normB = 0.0
    for ($i = 0; $i -lt 3; $i++) {
        $dot += $A[$i] * $B[$i]
        $normA += $A[$i] * $A[$i]
        $normB += $B[$i] * $B[$i]
    }
    return $dot / [Math]::Sqrt($normA * $normB)
}

function Decimal($Value) {
    return $Value.ToString('F3', [System.Globalization.CultureInfo]::GetCultureInfo('es-ES'))
}

$violin = @(0.80, 0.20, 0.10)
$guitarra = @(0.75, 0.25, 0.12)
$bicicleta = @(-0.30, 0.60, 0.80)
$blue = '#2563EB'
$green = '#00856A'
$purple = '#8A4CB8'

Text 'Embeddings: coordenadas en tres dimensiones' 55 35 43 '#172B49' $true
Text 'Los valores del vector sitúan cada token en un punto del espacio.' 58 98 25 '#586D88'
Box 40 170 920 815
Box 990 170 570 325
Box 990 520 570 215 '#EDF6FF'
Box 990 760 570 225 '#F0EBF9'

# Grid in the z=0 plane.
for ($i = -2; $i -le 5; $i++) {
    $x = $i / 5.0
    Line (Project $x 0 0) (Project $x 1 0)
}
for ($i = 0; $i -le 5; $i++) {
    $y = $i / 5.0
    Line (Project -0.4 $y 0) (Project 1 $y 0)
}

# Axes: the negative part of dimension 1 is needed for bicicleta.
$origin = Project 0 0 0
Line (Project -0.4 0 0) (Project 1.15 0 0) '#71819A' 2.8 $false $true
Line $origin (Project 0 1.2 0) '#71819A' 2.8 $false $true
Line $origin (Project 0 0 1.12) '#71819A' 2.8 $false $true

for ($i = -2; $i -le 5; $i++) {
    if ($i -eq 0) { continue }
    $t = $i / 5.0
    $at = Project $t 0 0
    Line @(($at[0] - 4), ($at[1] + 5)) @(($at[0] + 4), ($at[1] - 5)) '#71819A'
    Text ($t.ToString('0.0', [System.Globalization.CultureInfo]::GetCultureInfo('es-ES'))) ($at[0] + 5) ($at[1] + 8) 16 '#586D88'
}
for ($i = 1; $i -le 5; $i++) {
    $t = $i / 5.0
    $atY = Project 0 $t 0
    Text ($t.ToString('0.0', [System.Globalization.CultureInfo]::GetCultureInfo('es-ES'))) ($atY[0] - 35) ($atY[1] + 8) 16 '#586D88'
    $atZ = Project 0 0 $t
    Line @(($atZ[0] - 5), $atZ[1]) @(($atZ[0] + 5), $atZ[1]) '#71819A'
    Text ($t.ToString('0.0', [System.Globalization.CultureInfo]::GetCultureInfo('es-ES'))) ($atZ[0] - 40) ($atZ[1] - 11) 16 '#586D88'
}
Text 'Dimensión 1' 765 887 24 '#586D88' $true
Text 'Dimensión 2' 95 880 24 '#586D88' $true
Text 'Dimensión 3' 555 195 24 '#586D88' $true
Point $origin '#71819A' 5
Text 'Origen (0; 0; 0)' 560 600 18 '#586D88'

$items = @(
    @{ Values = $bicicleta; Color = $purple },
    @{ Values = $guitarra; Color = $green },
    @{ Values = $violin; Color = $blue }
)
foreach ($item in $items) {
    $v = $item.Values
    $at = Project $v[0] $v[1] $v[2]
    $base = Project $v[0] $v[1] 0
    Line $base $at $item.Color 1.5 $true
    Line $origin $at $item.Color 4 $false $true
    Point $at $item.Color
}

# Labels are offset from the points so the two nearby instruments stay distinct.
$atV = Project $violin[0] $violin[1] $violin[2]
$atG = Project $guitarra[0] $guitarra[1] $guitarra[2]
$atB = Project $bicicleta[0] $bicicleta[1] $bicicleta[2]
Line $atV @(790, 713) $blue 1.8
Text 'Violín' 790 675 29 $blue $true
Line $atG @(650, 822) $green 1.8
Text 'Guitarra' 550 825 29 $green $true
Line $atB @(190, 350) $purple 1.8
Text 'Bicicleta' 105 310 29 $purple $true
Text 'Líneas discontinuas: proyección sobre el plano de base.' 70 937 20 '#586D88'

Text 'COORDENADAS DEL EJEMPLO' 1018 192 23 '#172B49' $true
Text 'Violín' 1020 245 27 $blue $true
Text '[0,80; 0,20; 0,10]' 1020 284 26 $blue
Text 'Guitarra' 1020 327 27 $green $true
Text '[0,75; 0,25; 0,12]' 1020 366 26 $green
Text 'Bicicleta' 1020 409 27 $purple $true
Text '[−0,30; 0,60; 0,80]' 1190 410 25 $purple

Text '¿QUÉ MUESTRA EL MAPA?' 1018 540 23 '#172B49' $true
Text 'Los dos instrumentos quedan muy próximos.' 1018 582 22
Text 'Bicicleta ocupa una posición más alejada.' 1018 617 22
Text ('Distancia violín–guitarra: ' + (Decimal (Distance $violin $guitarra))) 1018 660 21 '#00856A' $true
Text ('Distancia violín–bicicleta: ' + (Decimal (Distance $violin $bicicleta))) 1018 694 21 $purple $true

Text '¿Y LA DIRECCIÓN DE LAS FLECHAS?' 1018 781 22 '#172B49' $true
Text 'El coseno compara su orientación.' 1018 823 22
Text ('Violín–guitarra: ' + (Decimal (Cosine $violin $guitarra))) 1018 865 24 '#00856A' $true
Text ('Violín–bicicleta: ' + (Decimal (Cosine $violin $bicicleta))) 1018 904 24 $purple $true
Text 'El coseno es semejanza, no probabilidad.' 1018 948 20 '#586D88'

Text 'Ejemplo inventado y reducido a tres dimensiones.' 58 1025 28 '#172B49' $true
Text 'Los ejes no significan «música», «tamaño» u otras propiedades fijas.' 58 1073 24 '#586D88'

$bitmap.Save($outputPath, [System.Drawing.Imaging.ImageFormat]::Png)
$g.Dispose()
$bitmap.Dispose()
Write-Output ([System.IO.Path]::GetFullPath($outputPath))
