﻿using System.Drawing;
using System.Threading.Tasks;
using Blazor.Extensions.Canvas.Canvas2D;

namespace SheddingCardGame.UI.Pages
{
    public interface IGameObject
    {
        bool IsVisible { get; set; }
        string Tag { get; set; }
        ValueTask Update(InputState inputState);
        ValueTask Render(Canvas2DContext context);
        bool IsHit(Point mousePosition);
    }
}