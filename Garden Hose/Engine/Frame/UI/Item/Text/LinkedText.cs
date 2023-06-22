using GardenHose.Engine.Frame.UI.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;


namespace GardenHose.Engine.Frame.UI.Item;


/* Represents a complex text box that is made from multiple text components. */
public class LinkedText : PositionalItem
{
    // Fields.
    public bool IsTypeable
    {
        get => IsTypeable;
        set
        {
            _isTypeable = value;
            if (_isTypeable) MainGame.Instance.Window.TextInput += OnUserType;
            else MainGame.Instance.Window.TextInput -= OnUserType;
        }
    }

    public bool WrapText = false;
    public bool AllowNewlines = true;

    public float MaxPixelsPerLine
    {
        get => _maxPixelsPerLine;
        set => _maxPixelsPerLine = Math.Max(1f, value);
    }
    public uint MaxLength = uint.MaxValue;

    public Vector2 TextOrigin = new();

    public string Text
    {
        get
        {
            StringBuilder Text = new(_components[0].Text.Length * _components.Length);
            foreach (var Component in _components) Text.Append(Component.Text);
            return Text.ToString();
        }
    }
    public TextComponent[] Components { get => _components; }


    // Private fields.
    private bool _isTypeable = false;
    private TextComponent[] _components;
    private float _maxPixelsPerLine = 300f;


    // Constructors.
    public LinkedText(TextComponentBuilder componentBuilder)
    {
        SetComponents(componentBuilder);
    }


    // Methods.
    public void SetTextOrigin(TextureOrigin origin)
    {
        TextOrigin.X = ((int)origin % 3) switch
        {
            0 => 0f,
            1 => TextSizePixels.X / 2f,
            2 => TextSizePixels.X,
            _ => 0f
        };

        TextOrigin.Y = ((int)origin / 3) switch
        {
            0 => 0f,
            1 => TextSizePixels.Y / 2f,
            2 => TextSizePixels.Y,
            _ => 0f
        };
    }


    // Private methods.
    private void FoldText()
    {
        StringBuilder NewText = new(_wrappedText);
        int StartIndex = 0; // Start of sentence.
        int EndIndex = 0; // End of sentence.
        int SpaceIndex = -1; // Index of last found space or tab. -1 means it doesn't exist.
        char Letter;
        float PartLength;

        for (int Index = 0; (Index < _wrappedText.Length) && (Index <= MaxLength); Index++, EndIndex++)
        {
            Letter = _wrappedText[Index];

            if (Letter is ' ' or '\t') SpaceIndex = Index;
            else if (Letter == '\n')
            {
                SpaceIndex = -1;
                StartIndex = Index + 1;
                EndIndex = StartIndex;
                continue;
            }

            PartLength = Font.FontAsset.MeasureString(_wrappedText.Substring(StartIndex, EndIndex - StartIndex)).X;
            if ((PartLength > MaxPixelsPerLine) && (SpaceIndex != -1))
            {
                NewText[SpaceIndex] = '\n';

                StartIndex = SpaceIndex + 1;
                EndIndex = Index;
                SpaceIndex = -1;
            }
        }

        _wrappedText = NewText.ToString();
    }


    // Inherited methods.
    public override void Draw()
    {
        base.Draw();

        if (IsVisible) GameFrame.DrawBatch.DrawString(
            _font.FontAsset,
            _wrappedText,
            RealPosition,
            RealColorMask,
            Rotation,
            TextOrigin,
            RealScale,
            SpriteEffects.None,
            1f);
    }


    // Private methods.
    private void SetComponents(TextComponentBuilder builder)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));
        if (builder.Components.Count == 0) throw new ArgumentException("TextComponentBuilder's list cannot be empty");

        if (MaxLength != uint.MaxValue) AdjustComponentLength(builder);
        if (WrapText) AddComponentNewlines(builder);
        AdjustComponentLayout(builder);

        _components = builder.Build();
    }

    private void AdjustComponentLength(TextComponentBuilder builder)
    {
        TextComponent Component;
        uint TotalLength = 0u;

        for (int i = 0; i < builder.Components.Count; i++)
        {
            Component = builder.Components[i];
            if ((TotalLength + (uint)Component.Text.Length) <= MaxLength) continue;

            Component.Text = Component.Text[0..(int)(MaxLength - TotalLength - 1)];

            if (i < builder.Components.Count - 1)
            {
                i++;
                builder.Components.RemoveRange(i, builder.Components.Count - i);
            }
                
            return;
        }
    }

    private void AddComponentNewlines(TextComponentBuilder builder)
    {

        foreach (var Component in builder.Components) 
        {

        }
    }

    private void AdjustComponentLayout(TextComponentBuilder builder)
    {
        List<TextComponent> NewComponents = new(builder.Components.Count);
        TextComponent BaseComponent = builder.Components[0];
        TextComponent Component;
        float PixelsXUntilThis = 0f;
        int NewLineIndex;
        int CurrentLine = 0;

        for (int i = 1; i < builder.Components.Count; i++)
        {
            Component = builder.Components[i];
            NewLineIndex = Component.Text.IndexOf('\n');
            

            // Adjust position.
            if (i > 0)
            {
                Vector2 NewPosition;

                NewPosition.X = BaseComponent.Position.X + PixelsXUntilThis;
                NewPosition.Y = (CurrentLine * BaseComponent.TextFont.FontAsset.LineSpacing) + BaseComponent.Position.Y;

                Component.Position = NewPosition;
            }


            // Split component.
            if (NewLineIndex == -1)
            {
                NewComponents.Add(Component);
                PixelsXUntilThis += Component.TextSizePixels.X;
                continue;
            }

            PixelsXUntilThis = 0f;
            CurrentLine++;

            List<TextComponent> SeparatedComponents = new(2)
            {
                new(Component.Text[0..(NewLineIndex - 1)], Component.TextFont),
            };
            if (NewLineIndex > Component.Text.Length - 1) SeparatedComponents.Add(
                new(Component.Text[(NewLineIndex + 1)..^1], Component.TextFont));

            builder.Components.InsertRange(i + 1, SeparatedComponents);
            builder.Components.RemoveAt(i);
            i++;
        }

        builder.Components.Clear();
        builder.Components.AddRange(NewComponents);
    }

    private void OnUserType(object sender, TextInputEventArgs args)
    {
        //if (args.Key == Keys.Back)
        //{
        //    Text = Text.Length == 0 ? String.Empty : _realText.Substring(0, _realText.Length - 1);
        //}

        //if (_realText.Length >= MaxLength) return;

        //string Typed = null;
        //if (args.Character is >= ' ' and <= '~')
        //{
        //    Typed = args.Character.ToString();
        //}
        //else if (args.Key == Keys.Enter)
        //{
        //    if (AllowNewlines) Typed = "\n";
        //}
        //else if (args.Key == Keys.Tab) Typed = "    ";
        //else
        //{
        //    Logger.Warning($"Couldn't resolve pressed key for {nameof(LinkedText)}" +
        //            $"\nKey pressed: \"{args.Key}\", Character: \"{args.Character}\", Font: \"{Font.Name}\"");
        //    return;
        //}

        //if (LimitLength &&
        //    (Font.FontAsset.MeasureString(Typed).X
        //    + Font.FontAsset.MeasureString(_realText).X
        //    > _maxPixelsPerLine)) return;

        //Text = _realText + Typed;
    }
}
