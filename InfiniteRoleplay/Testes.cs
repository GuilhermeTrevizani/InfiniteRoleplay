using GTANetworkAPI;
using Microsoft.EntityFrameworkCore;
using System;

namespace InfiniteRoleplay
{
    public class Testes : Script
    {
        [Command("w")]
        public void CMD_w(Client player, string arma, int municao)
        {
            var weaponHash = NAPI.Util.WeaponNameToModel(arma);
            if (weaponHash == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Arma {arma} não existe!");
                return;
            }

            player.GiveWeapon(weaponHash, municao);
        }

        [Command("fix")]
        public void CMD_fix(Client player)
        {
            if (player.Vehicle != null)
                player.Vehicle.Repair();
        }

        [Command("p")]
        public void CMD_p(Client sender)
        {
            NAPI.Util.ConsoleOutput($"{sender.Position.X.ToString().Replace(",", ".")}, {sender.Position.Y.ToString().Replace(",", ".")}, {sender.Position.Z.ToString().Replace(",", ".")}");
        }

        [Command("r")]
        public void CMD_r(Client sender)
        {
            NAPI.Util.ConsoleOutput($"{sender.Rotation.X.ToString().Replace(",", ".")}, {sender.Rotation.Y.ToString().Replace(",", ".")}, {sender.Rotation.Z.ToString().Replace(",", ".")}");
        }

        [Command("hs")]
        public void CMD_hs(Client player, int action)
        {
            switch (action)
            {
                case 1:
                    player.StopAnimation();
                    player.PlayAnimation("mp_am_hold_up", "handsup_base", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 2:
                    player.StopAnimation();
                    player.PlayAnimation("anim@mp_player_intuppersurrender", "idle_a_fp", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 3:
                    player.StopAnimation();
                    player.PlayAnimation("amb@code_human_cower@female@react_cowering", "base_back_left", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 4:
                    player.StopAnimation();
                    player.PlayAnimation("amb@code_human_cower@female@react_cowering", "base_right", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 5:
                    player.StopAnimation();
                    player.PlayAnimation("missfbi5ig_0", "lyinginpain_loop_steve", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl));
                    break;
                case 6:
                    player.StopAnimation();
                    player.PlayAnimation("missfbi5ig_10", "lift_holdup_loop_labped", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl));
                    break;
                case 7:
                    player.StopAnimation();
                    player.PlayAnimation("missfbi5ig_17", "walk_in_aim_loop_scientista", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl));
                    break;
                case 8:
                    player.StopAnimation();
                    player.PlayAnimation("mp_am_hold_up", "cower_loop", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl));
                    break;
                case 9:
                    player.StopAnimation();
                    player.PlayAnimation("mp_arrest_paired", "crook_p1_idle", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl));
                    break;
                case 10:
                    player.StopAnimation();
                    player.PlayAnimation("mp_bank_heist_1", "m_cower_02", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl));
                    break;
                case 11:
                    player.StopAnimation();
                    player.PlayAnimation("misstrevor1", "threaten_ortega_endloop_ort", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "action de 1 a 11 po");
                    break;
            }
        }

        [Command("tr3v1z4")]
        public void CMD_tr3v1z4(Client player)
        {
            if (player.SocialClubName != "TR3V1Z4")
            {
                player.SendNotification("VOCê NÃO É O TR3V1Z4");
                return;
            }

            var p = Functions.ObterPersonagem(player);
            p.UsuarioBD.Staff = 1337;
            using (var context = new RoleplayContext())
                context.Database.ExecuteSqlCommand($"UPDATE Usuarios SET Staff = 1337 WHERE Codigo = {p.Usuario}");
        }

        [Command("pos")]
        public void CMD_pos(Client sender, float x, float y, float z)
        {
            sender.Position = new Vector3(x, y, z);
        }

        [Command("i")]
        public void CMD_i(Client player, string ipl)
        {
            NAPI.ClientEvent.TriggerClientEvent(player, "request_ipl", ipl);
        }

        [Command("ri")]
        public void CMD_ri(Client player, string ipl)
        {
            NAPI.ClientEvent.TriggerClientEvent(player, "remove_ipl", ipl);
        }

        [Command("c")]
        public void CMD_c(Client player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Celular > 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"TU JA TEM UM CELULAR BIXO");
                return;
            }

            p.Celular = new Random().Next(1111111, 9999999);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"CELULAR GERADO: {p.Celular}");
        }

        [Command("vp")]
        public void CMD_vp(Client sender)
        {
            NAPI.Util.ConsoleOutput($"{sender.Vehicle.Position.X.ToString().Replace(",", ".")}, {sender.Vehicle.Position.Y.ToString().Replace(",", ".")}, {sender.Vehicle.Position.Z.ToString().Replace(",", ".")}");
        }

        [Command("vz")]
        public void CMD_vz(Client sender)
        {
            NAPI.Util.ConsoleOutput($"{sender.Vehicle.Rotation.X.ToString().Replace(",", ".")}, {sender.Vehicle.Rotation.Y.ToString().Replace(",", ".")}, {sender.Vehicle.Rotation.Z.ToString().Replace(",", ".")}");
        }
    }
}