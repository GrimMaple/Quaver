﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quaver.API.Enums;
using Quaver.Graphics;
using Quaver.Graphics.Base;
using Quaver.Graphics.Buttons;
using Quaver.Graphics.Graphing;
using Quaver.Graphics.Sprites;
using Quaver.Graphics.Text;
using Quaver.Helpers;
using Quaver.Main;

namespace Quaver.States.Results.UI.Data
{
    internal class ResultsScoreStatistics : HeaderedSprite
    {
        /// <summary>
        ///     Reference to the score processor that was played.
        /// </summary>
        private ResultsScreen Screen { get; }

        /// <summary>
        ///     The list of stat containers that we currently have.
        /// </summary>
        private List<StatisticContainer> StatsContainers { get; }

        /// <summary>
        ///     The currently selected container.
        /// </summary>
        private int SelectedContainer { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="screen"></param>
        /// <param name="stats"></param>
        public ResultsScoreStatistics(ResultsScreen screen, List<StatisticContainer> stats = null)
                                    : base(new Vector2(screen.UI.JudgementBreakdown.SizeX, screen.UI.JudgementBreakdown.SizeY),
                                        "Statistics", Fonts.AllerRegular16, 0.90f, Alignment.MidCenter, 50, Colors.DarkGray)
        {
            Screen = screen;
            PosX = SizeX / 2f + 10;

            StatsContainers = stats;

            Content = CreateContent();
            Content.Parent = this;
            Content.PosY = 50;
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <returns></returns>
        protected sealed override Sprite CreateContent()
        {
            var content = new Sprite
            {
                Parent = this,
                Size = new UDim2D(ContentSize.X, ContentSize.Y),
                Tint = Color.Black,
                Alpha = 0.45f
            };

            // If there aren't any stats to display, then display a message to the user.
            if (StatsContainers.Count == 0)
            {
                // ReSharper disable once ObjectCreationAsStatement
                new SpriteText
                {
                    Parent = content,
                    Alignment = Alignment.MidCenter,
                    TextAlignment = Alignment.MidCenter,
                    Font = Fonts.Exo2Regular24,
                    Text = "No statistics available. Play the map or watch the replay to retrieve them.",
                    TextScale = 0.50f
                };

                return content;
            }

            CreateTabs(content);

            return content;
        }

        /// <summary>
        ///     Creates the tabs for each individual container we have.
        /// </summary>
        /// <param name="content"></param>
        private void CreateTabs(Drawable content)
        {
            var buttonContainer = new Sprite()
            {
                Parent = content,
                Alpha = 1f,
                Size = new UDim2D(content.SizeX - 75, 35),
                Alignment = Alignment.BotCenter,
                Tint = Colors.DarkGray,
                PosY = -5
            };

            for (var i = 0; i < StatsContainers.Count; i++)
            {
                // Set the parent of the content to the actual content, so that we can display it.
                // We want to give full control here over sprite creation, so we don't change it at all.
                StatsContainers[i].Content.Parent = content;
                StatsContainers[i].Content.SetChildrenAlpha = true;

                // Create Tab Button
                StatsContainers[i].Button = new TextButton(new Vector2(150, buttonContainer.SizeY), StatsContainers[i].Name.ToUpper())
                {
                    Parent = buttonContainer,
                    Alignment = Alignment.MidLeft,
                    Alpha = 1f,
                    TextSprite =
                    {
                        Font = Fonts.AssistantRegular16,
                        TextScale = 0.75f,
                        TextColor = Color.Black
                    },
                };

                var btn = StatsContainers[i].Button;

                if (i == 0)
                {
                    btn.Tint = Colors.SecondaryAccent;
                    btn.TextSprite.TextColor = Color.Black;
                }
                else
                {
                    btn.Tint = Color.Black;
                    btn.Alpha = 0.50f;
                    btn.TextSprite.TextColor = Color.White;

                    // Set button position
                    var sizePer = btn.SizeX / StatsContainers.Count;
                    btn.PosX = (btn.SizeX) * i;

                    // Make sure that the the alpha of inactive ones are invisible.
                    StatsContainers[i].Content.Alpha = 0;
                }

                btn.Clicked += (sender, args) =>
                {
                    if ((TextButton) sender == StatsContainers[SelectedContainer].Button)
                        return;

                    for (var j = 0; j < StatsContainers.Count; j++)
                    {
                        if (StatsContainers[j].Button != (TextButton) sender)
                            continue;

                        SelectedContainer = j;
                        break;
                    }
                };
            }
        }

        /// <inheritdoc />
        ///  <summary>
        ///  </summary>
        ///  <param name="dt"></param>
        internal override void Update(double dt)
        {
            for (var i = 0; i < StatsContainers.Count; i++)
            {
                var container = StatsContainers[i];

                if (i != SelectedContainer)
                {
                    // Make sure button is black and unfocused.
                    container.Button.FadeToColor(container.Button.IsTrulyHovered ? Colors.SecondaryAccentInactive : Color.Black, dt, 60);
                    container.Button.Alpha = GraphicsHelper.Tween(0.5f, container.Button.Alpha, Math.Min(dt / 60, 1));
                    container.Button.TextSprite.TextColor = Color.White;

                    // Make sure that the contaier content itself is hidden
                    container.Content.Alpha = GraphicsHelper.Tween(0, container.Content.Alpha, Math.Min(dt / 120, 1));
                }
                else
                {
                    // Make sure button is yellow and focused.
                    container.Button.Alpha = GraphicsHelper.Tween(1, container.Button.Alpha, Math.Min(dt / 60, 1));
                    container.Button.FadeToColor(Colors.SecondaryAccent, dt, 60);
                    container.Button.TextSprite.TextColor = Color.Black;

                    // Make sure that the contaier content itself is visible
                    container.Content.Alpha = GraphicsHelper.Tween(1, container.Content.Alpha, Math.Min(dt / 120, 1));
                }
            }

            base.Update(dt);
        }
    }
}