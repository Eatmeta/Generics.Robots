using System;
using System.Collections.Generic;

namespace Generics.Robots
{
    public interface IRobotAI<out TMoveCommand>
    {
        TMoveCommand GetCommand();
    }

    public class ShooterAI : IRobotAI<ShooterCommand>
    {
        private int _counter = 1;

        public ShooterCommand GetCommand()
        {
            return ShooterCommand.ForCounter(_counter++);
        }
    }

    public class BuilderAI : IRobotAI<IMoveCommand>
    {
        private int _counter = 1;

        public IMoveCommand GetCommand()
        {
            return BuilderCommand.ForCounter(_counter++);
        }
    }

    public interface IDevice<in TMoveCommand>
    {
        string ExecuteCommand(TMoveCommand command);
    }

    public class Mover : IDevice<IMoveCommand>
    {
        public string ExecuteCommand(IMoveCommand command)
        {
            if (command == null)
                throw new ArgumentException();
            return $"MOV {command.Destination.X}, {command.Destination.Y}";
        }
    }

    public class ShooterMover : IDevice<IShooterMoveCommand>
    {
        public string ExecuteCommand(IShooterMoveCommand command)
        {
            if (command == null)
                throw new ArgumentException();
            var hide = command.ShouldHide ? "YES" : "NO";
            return $"MOV {command.Destination.X}, {command.Destination.Y}, USE COVER {hide}";
        }
    }

    public static class Robot
    {
        public static Robot<TMoveCommand> Create<TMoveCommand>(IRobotAI<TMoveCommand> ai, IDevice<TMoveCommand> device)
        {
            return new Robot<TMoveCommand>(ai, device);
        }
    }

    public class Robot<TMoveCommand>
    {
        private readonly IRobotAI<TMoveCommand> _ai;
        private readonly IDevice<TMoveCommand> _device;

        public Robot(IRobotAI<TMoveCommand> ai, IDevice<TMoveCommand> executor)
        {
            _ai = ai;
            _device = executor;
        }

        public IEnumerable<string> Start(int steps)
        {
            for (var i = 0; i < steps; i++)
            {
                var command = _ai.GetCommand();
                if (command == null)
                    break;
                yield return _device.ExecuteCommand(command);
            }
        }
    }
}