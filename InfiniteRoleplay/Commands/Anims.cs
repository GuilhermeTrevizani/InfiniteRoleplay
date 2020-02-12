using GTANetworkAPI;

namespace InfiniteRoleplay.Commands
{
    public class Anims : Script
    {
        [Command("stopanim", "!{#febd0c}USO:~w~ /stopanim", Alias = "sa")]
        public void CMD_stopanim(Client player) => Functions.ChecarAnimacoes(player, true);

        [Command("handsup", "!{#febd0c}USO:~w~ /hs (tipo [1-13])", Alias = "hs")]
        public void CMD_hs(Client player, int tipo)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    player.PlayAnimation("mp_am_hold_up", "handsup_base", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 2:
                    player.PlayAnimation("anim@mp_player_intuppersurrender", "idle_a_fp", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 3:
                    player.PlayAnimation("amb@code_human_cower@female@react_cowering", "base_back_left", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 4:
                    player.PlayAnimation("amb@code_human_cower@female@react_cowering", "base_right", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 5:
                    player.PlayAnimation("missfbi5ig_0", "lyinginpain_loop_steve", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl));
                    break;
                case 6:
                    player.PlayAnimation("missfbi5ig_10", "lift_holdup_loop_labped", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl));
                    break;
                case 7:
                    player.PlayAnimation("missfbi5ig_17", "walk_in_aim_loop_scientista", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl));
                    break;
                case 8:
                    player.PlayAnimation("mp_am_hold_up", "cower_loop", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl));
                    break;
                case 9:
                    player.PlayAnimation("mp_arrest_paired", "crook_p1_idle", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl));
                    break;
                case 10:
                    player.PlayAnimation("mp_bank_heist_1", "m_cower_02", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl));
                    break;
                case 11:
                    player.PlayAnimation("misstrevor1", "threaten_ortega_endloop_ort", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl));
                    break;
                case 12:
                    player.PlayAnimation("missminuteman_1ig_2", "handsup_base", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl));
                    break;
                case 13:
                    player.PlayAnimation("anim@mp_player_intincarsurrenderstd@ds@", "idle_a", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 13.");
                    break;
            }
        }

        [Command("crossarms", "!{#febd0c}USO:~w~ /crossarms (tipo [1-2])")]
        public void CMD_crossarms(Client player, int tipo)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    player.PlayAnimation("amb@world_human_hang_out_street@male_c@base", "base", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 2:
                    player.PlayAnimation("missheistdockssetup1ig_10@base", "talk_pipe_base_worker2", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 2.");
                    break;
            }
        }

        [Command("smoke", "!{#febd0c}USO:~w~ /smoke (tipo [1-3])")]
        public void CMD_smoke(Client player, int tipo)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    player.PlayAnimation("amb@world_human_smoking@male@male_a@enter", "enter", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 2:
                    player.PlayAnimation("amb@world_human_smoking@male@male_a@base", "base", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 3:
                    player.PlayAnimation("amb@world_human_smoking@male@male_a@exit", "exit", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 3.");
                    break;
            }
        }

        [Command("lean", "!{#febd0c}USO:~w~ /lean (tipo [1-7])")]
        public void CMD_lean(Client player, int tipo)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    player.PlayAnimation("amb@world_human_leaning@male@wall@back@hands_together@base", "base", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 2:
                    player.PlayAnimation("amb@world_human_leaning@male@wall@back@foot_up@base", "base", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 3:
                    player.PlayAnimation("amb@world_human_leaning@male@wall@back@legs_crossed@base", "base", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 4:
                    player.PlayAnimation("misscarstealfinale", "packer_idle_base_trevor", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 5:
                    player.PlayAnimation("switch@michael@marina", "loop", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 6:
                    player.PlayAnimation("switch@michael@pier", "pier_lean_smoke_idle", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 7:
                    player.PlayAnimation("switch@michael@sitting_on_car_premiere", "sitting_on_car_premiere_loop_player", (int)(Constants.AnimationFlags.Loop));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 7.");
                    break;
            }
        }

        [Command("police", "!{#febd0c}USO:~w~ /police (tipo [1-6])")]
        public void CMD_police(Client player, int tipo)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    player.PlayAnimation("amb@code_human_police_crowd_control@base", "base", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 2:
                    player.PlayAnimation("amb@code_human_police_crowd_control@idle_a", "idle_a", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 3:
                    player.PlayAnimation("amb@code_human_police_crowd_control@idle_b", "idle_d", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 4:
                    player.PlayAnimation("amb@code_human_police_investigate@base", "base", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 5:
                    player.PlayAnimation("amb@code_human_police_investigate@idle_a", "idle_a", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 6:
                    player.PlayAnimation("amb@code_human_police_investigate@idle_b", "idle_f", (int)(Constants.AnimationFlags.Loop));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 6.");
                    break;
            }
        }

        [Command("incar", "!{#febd0c}USO:~w~ /incar (tipo [1-3])")]
        public void CMD_incar(Client player, int tipo)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    player.PlayAnimation("amb@incar@male@patrol@ds@idle_b", "idle_d", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 2:
                    player.PlayAnimation("amb@incar@male@patrol@ds@base", "base", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 3:
                    player.PlayAnimation("amb@incar@male@patrol@ps@idle_a", "idle_a", (int)(Constants.AnimationFlags.Loop));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 3.");
                    break;
            }
        }

        [Command("pushups", "!{#febd0c}USO:~w~ /pushups (tipo [1-4])")]
        public void CMD_pushups(Client player, int tipo)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    player.PlayAnimation("amb@world_human_push_ups@male@enter", "enter", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break;
                case 2:
                    player.PlayAnimation("amb@world_human_push_ups@male@idle_a", "idle_a", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 3:
                    player.PlayAnimation("amb@world_human_push_ups@male@base", "base", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 4:
                    player.PlayAnimation("amb@world_human_push_ups@male@exit", "exit", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 4.");
                    break;
            }
        }

        [Command("situps", "!{#febd0c}USO:~w~ /situps (tipo [1-4])")]
        public void CMD_situps(Client player, int tipo)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    player.PlayAnimation("amb@world_human_sit_ups@male@enter", "enter", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break;
                case 2:
                    player.PlayAnimation("amb@world_human_sit_ups@male@idle_a", "idle_a", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 3:
                    player.PlayAnimation("amb@world_human_sit_ups@male@base", "base", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 4:
                    player.PlayAnimation("amb@world_human_sit_ups@male@exit", "exit", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 4.");
                    break;
            }
        }

        [Command("blunt", "!{#febd0c}USO:~w~ /blunt (tipo [1-2])")]
        public void CMD_blunt(Client player, int tipo)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    player.PlayAnimation("amb@world_human_smoking_pot@male@base", "base", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 2:
                    player.PlayAnimation("amb@world_human_smoking_pot@male@idle_a", "idle_a", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 2.");
                    break;
            }
        }

        [Command("afishing", "!{#febd0c}USO:~w~ /afishing (tipo [1-3])")]
        public void CMD_afishing(Client player, int tipo)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    player.PlayAnimation("amb@world_human_stand_fishing@base", "base", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 2:
                    player.PlayAnimation("amb@world_human_stand_fishing@idle_a", "idle_a", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 3:
                    player.PlayAnimation("amb@world_human_stand_fishing@idle_a", "idle_c", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 3.");
                    break;
            }
        }

        [Command("acop", "!{#febd0c}USO:~w~ /acop")]
        public void CMD_acop(Client player)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            player.PlayAnimation("amb@world_human_cop_idles@male@base", "base", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
        }

        [Command("idle", "!{#febd0c}USO:~w~ /idle (tipo [1-3])")]
        public void CMD_idle(Client player, int tipo)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    player.PlayAnimation("amb@world_human_drug_dealer_hard@male@base", "base", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 2:
                    player.PlayAnimation("amb@world_human_drug_dealer_hard@male@idle_a", "idle_a", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 3:
                    player.PlayAnimation("amb@world_human_drug_dealer_hard@male@idle_b", "idle_d", (int)(Constants.AnimationFlags.Loop));
                    break; 
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 3.");
                    break;
            }
        }

        [Command("barra", "!{#febd0c}USO:~w~ /barra (tipo [1-3])")]
        public void CMD_barra(Client player, int tipo)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    player.PlayAnimation("amb@prop_human_muscle_chin_ups@male@enter", "enter", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break; 
                case 2:
                    player.PlayAnimation("amb@prop_human_muscle_chin_ups@male@base", "base", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 3:
                    player.PlayAnimation("amb@prop_human_muscle_chin_ups@male@exit", "exit_flee", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break; 
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 3.");
                    break;
            }
        }

        [Command("kneel", "!{#febd0c}USO:~w~ /kneel")]
        public void CMD_kneel(Client player)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            player.PlayAnimation("amb@medic@standing@tendtodead@base", "base", (int)(Constants.AnimationFlags.Loop));
        }

        [Command("revistarc", "!{#febd0c}USO:~w~ /revistarc")]
        public void CMD_revistarc(Client player)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            player.PlayAnimation("amb@medic@standing@tendtodead@idle_a", "idle_a", (int)(Constants.AnimationFlags.Loop));
        }

        [Command("ajoelhar", "!{#febd0c}USO:~w~ /ajoelhar (tipo [1-4])")]
        public void CMD_ajoelhar(Client player, int tipo)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    player.PlayAnimation("amb@medic@standing@kneel@enter", "enter", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break;
                case 2:
                    player.PlayAnimation("amb@medic@standing@kneel@base", "base", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break; 
                case 3:
                    player.PlayAnimation("amb@medic@standing@kneel@idle_a", "idle_a", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break;
                case 4:
                    player.PlayAnimation("amb@medic@standing@kneel@exit", "exit_flee", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 4.");
                    break;
            }
        }

        [Command("drink", "!{#febd0c}USO:~w~ /drink (tipo [1-3])")]
        public void CMD_drink(Client player, int tipo)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    player.PlayAnimation("amb@world_human_drinking@beer@male@base", "base", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 2:
                    player.PlayAnimation("amb@world_human_drinking@coffee@male@base", "base", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 3:
                    player.PlayAnimation("amb@world_human_drinking@coffee@female@base", "base", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 3.");
                    break;
            }
        }

        [Command("morto", "!{#febd0c}USO:~w~ /morto (tipo [1-2])")]
        public void CMD_morto(Client player, int tipo)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    player.PlayAnimation("missfinale_c1@", "lying_dead_player0", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 2:
                    player.PlayAnimation("misslamar1dead_body", "dead_idle", (int)(Constants.AnimationFlags.Loop));
                    break; 
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 2.");
                    break;
            }
        }

        [Command("gsign", "!{#febd0c}USO:~w~ /gsign (tipo [1-2])")]
        public void CMD_gsign(Client player, int tipo)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    player.PlayAnimation("mp_player_int_uppergang_sign_a", "mp_player_int_gang_sign_a", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 2:
                    player.PlayAnimation("mp_player_int_uppergang_sign_b", "mp_player_int_gang_sign_b", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 2.");
                    break;
            }
        }

        [Command("hurry", "!{#febd0c}USO:~w~ /hurry (tipo [1-2])")]
        public void CMD_hurry(Client player, int tipo)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    player.PlayAnimation("missheist_agency3aig_18", "say_hurry_up_a", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 2:
                    player.PlayAnimation("missheist_agency3aig_18", "say_hurry_up_b", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 2.");
                    break;
            }
        }

        [Command("cair", "!{#febd0c}USO:~w~ /cair (tipo [1-2])")]
        public void CMD_cair(Client player, int tipo)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    player.PlayAnimation("mp_bank_heist_1", "prone_l_loop", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 2:
                    player.PlayAnimation("mp_bank_heist_1", "prone_r_loop", (int)(Constants.AnimationFlags.Loop));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 2.");
                    break;
            }
        }

        [Command("wsup", "!{#febd0c}USO:~w~ /wsup (tipo [1-2])")]
        public void CMD_wsup(Client player, int tipo)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    player.PlayAnimation("rcmme_amanda1", "pst_arrest_loop_owner", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 2:
                    player.PlayAnimation("missheist_agency2aig_12", "look_at_plan_b", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 2.");
                    break;
            }
        }

        [Command("render", "!{#febd0c}USO:~w~ /render (tipo [1-2])")]
        public void CMD_render(Client player, int tipo)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    player.PlayAnimation("random@arrests@busted", "idle_c", (int)(Constants.AnimationFlags.Loop));
                    break; 
                case 2:
                    player.PlayAnimation("random@arrests", "kneeling_arrest_idle", (int)(Constants.AnimationFlags.Loop));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 2.");
                    break;
            }
        }

        [Command("mirar", "!{#febd0c}USO:~w~ /mirar (tipo [1-2])")]
        public void CMD_mirar(Client player, int tipo)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    player.PlayAnimation("combat@aim_variations@arrest", "cop_med_arrest_01", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 2:
                    player.PlayAnimation("missfbi2", "franklin_sniper_crouch", (int)(Constants.AnimationFlags.Loop));
                    break; 
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 2.");
                    break;
            }
        }

        [Command("sentar", "!{#febd0c}USO:~w~ /sentar (tipo [1-8])")]
        public void CMD_sentar(Client player, int tipo)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    player.PlayAnimation("switch@michael@sitting", "idle", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break; 
                case 2:
                    player.PlayAnimation("switch@michael@tv_w_kids", "001520_02_mics3_14_tv_w_kids_idle_mic", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break; 
                case 3:
                    player.PlayAnimation("switch@michael@on_sofa", "base_michael", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break; 
                case 4:
                    player.PlayAnimation("safe@franklin@ig_13", "base", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break; 
                case 5:
                    player.PlayAnimation("switch@michael@bench", "bench_on_phone_idle", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break;
                case 6:
                    player.PlayAnimation("switch@michael@parkbench_smoke_ranger", "parkbench_smoke_ranger_loop", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break;
                case 7:
                    player.PlayAnimation("switch@michael@smoking2", "loop", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break; 
                case 8:
                    player.PlayAnimation("switch@michael@tv_w_kids", "001520_02_mics3_14_tv_w_kids_idle_jmy", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break; 
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 8.");
                    break;
            }
        }

        [Command("dormir", "!{#febd0c}USO:~w~ /dormir (tipo [1-2])")]
        public void CMD_dormir(Client player, int tipo)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    player.PlayAnimation("switch@franklin@napping", "002333_01_fras_v2_10_napping_idle", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break; 
                case 2:
                    player.PlayAnimation("switch@trevor@dumpster", "002002_01_trvs_14_dumpster_idle", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 2.");
                    break;
            }
        }

        [Command("pixar", "!{#febd0c}USO:~w~ /pixar (tipo [1-2])")]
        public void CMD_pixar(Client player, int tipo)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    player.PlayAnimation("switch@franklin@lamar_tagging_wall", "lamar_tagging_wall_loop_lamar", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                case 2:
                    player.PlayAnimation("switch@franklin@lamar_tagging_wall", "lamar_tagging_exit_loop_lamar", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 2.");
                    break;
            }
        }

        [Command("sexo", "!{#febd0c}USO:~w~ /sexo (tipo [1-5])")]
        public void CMD_sexo(Client player, int tipo)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    player.PlayAnimation("switch@trevor@garbage_food", "loop_trevor", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 2:
                    player.PlayAnimation("switch@trevor@head_in_sink", "trev_sink_idle", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 3:
                    player.PlayAnimation("switch@trevor@mocks_lapdance", "001443_01_trvs_28_idle_stripper", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 4:
                    player.PlayAnimation("misscarsteal2pimpsex", "shagloop_hooker", (int)(Constants.AnimationFlags.Loop));
                    break;
                case 5:
                    player.PlayAnimation("misscarsteal2pimpsex", "shagloop_pimp", (int)(Constants.AnimationFlags.Loop));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 5.");
                    break;
            }
        }

        [Command("jogado", "!{#febd0c}USO:~w~ /jogado (tipo [1-3])")]
        public void CMD_jogado(Client player, int tipo)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    player.PlayAnimation("switch@trevor@slouched_get_up", "trev_slouched_get_up_idle", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break;
                case 2:
                    player.PlayAnimation("switch@trevor@naked_island", "loop", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break;
                case 3:
                    player.PlayAnimation("rcm_barry3", "barry_3_sit_loop", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 3.");
                    break;
            }
        }

        [Command("reparando", "!{#febd0c}USO:~w~ /reparando (tipo [1-2])")]
        public void CMD_reparando(Client player, int tipo)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    player.PlayAnimation("switch@trevor@garbage_food", "loop_trevor", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break;
                case 2:
                    player.PlayAnimation("switch@trevor@puking_into_fountain", "trev_fountain_puke_loop", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 2.");
                    break;
            }
        }

        [Command("luto", "!{#febd0c}USO:~w~ /luto (tipo [1-2])")]
        public void CMD_luto(Client player, int tipo)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            switch (tipo)
            {
                case 1:
                    player.PlayAnimation("switch@michael@rejected_entry", "001396_01_mics3_6_rejected_entry_idle_bouncer", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break;
                case 2:
                    player.PlayAnimation("switch@michael@talks_to_guard", "001393_02_mics3_3_talks_to_guard_idle_guard", (int)(Constants.AnimationFlags.StopOnLastFrame));
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 2.");
                    break;
            }
        }

        [Command("bar", "!{#febd0c}USO:~w~ /bar")]
        public void CMD_bar(Client player)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            player.PlayAnimation("switch@trevor@bar", "exit_loop_bartender", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
        }

        [Command("necessidades", "!{#febd0c}USO:~w~ /necessidades")]
        public void CMD_necessidades(Client player)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            player.PlayAnimation("switch@trevor@on_toilet", "trev_on_toilet_loop", (int)(Constants.AnimationFlags.StopOnLastFrame));
        }

        [Command("meth", "!{#febd0c}USO:~w~ /meth")]
        public void CMD_meth(Client player)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            player.PlayAnimation("switch@trevor@trev_smoking_meth", "trev_smoking_meth_loop", (int)(Constants.AnimationFlags.StopOnLastFrame));
        }

        [Command("mijar", "!{#febd0c}USO:~w~ /mijar")]
        public void CMD_mijar(Client player)
        {
            if (!Functions.ChecarAnimacoes(player))
                return;

            player.PlayAnimation("misscarsteal2peeing", "peeing_loop", (int)(Constants.AnimationFlags.Loop | Constants.AnimationFlags.AllowPlayerControl | Constants.AnimationFlags.OnlyAnimateUpperBody));
        }
    }
}