using System;
using System.Collections.Generic;

namespace Generics.Robots
{
    public abstract class RobotAI
    {
        public abstract object GetCommand();
    }

    public class ShooterAI : RobotAI
    {
        private int _counter = 1;

        public override object GetCommand()
        {
            return ShooterCommand.ForCounter(_counter++);
        }
    }

    public class BuilderAI : RobotAI
    {
        private int _counter = 1;

        public override object GetCommand()
        {
            return BuilderCommand.ForCounter(_counter++);
        }
    }

    public abstract class Device
    {
        public abstract string ExecuteCommand(object command);
    }

    public class Mover : Device
    {
        public override string ExecuteCommand(object _command)
        {
            var command = _command as IMoveCommand;
            if (command == null)
                throw new ArgumentException();
            return $"MOV {command.Destination.X}, {command.Destination.Y}";
        }
    }

    public class ShooterMover : Device
    {
        public override string ExecuteCommand(object _command)
        {
            var command = _command as IShooterMoveCommand;
            if (command == null)
                throw new ArgumentException();
            var hide = command.ShouldHide ? "YES" : "NO";
            return $"MOV {command.Destination.X}, {command.Destination.Y}, USE COVER {hide}";
        }
    }

    public class Robot
    {
        private readonly RobotAI _ai;
        private readonly Device _device;

        public Robot(RobotAI ai, Device executor)
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

        public static Robot Create<TCommand>(RobotAI ai, Device executor)
        {
            return new Robot(ai, executor);
        }
    }
}
