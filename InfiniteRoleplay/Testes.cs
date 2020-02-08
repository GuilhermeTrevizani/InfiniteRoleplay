using GTANetworkAPI;
using Newtonsoft.Json;

namespace InfiniteRoleplay
{
    public class Testes : Script
    {
        #region Not Implemented Yet
        /*
        [Command("ame", GreedyArg = true)]
        public void CMD_ame(Client player, string mensagem = "")
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            if (p.TextLabel != null)
                p.TextLabel.Delete();

            p.TextLabel = NAPI.TextLabel.CreateTextLabel($"{p.NomeIC} {mensagem}", new Vector3(0.0f, 0.0f, 0.0f), 50.0f, 0.5f, 4, new Color(201, 90, 0, 255));
            NAPI.ClientEvent.TriggerClientEvent(player, "attachEntityToEntity", p.TextLabel, player, 31086, new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, 0.0f));

            NAPI.Task.Run(() =>
            {
                if (p.TextLabel != null)
                {
                    p.TextLabel.Delete();
                    p.TextLabel = null;
                }
            }, delayTime: 5000);
        }
        */
        #endregion

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

        [Command("c")]
        public void CMD_c(Client player, string veh)
        {
            var weaponHash = NAPI.Util.VehicleNameToModel(veh);
            if (weaponHash == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"VEH não existe!");
                return;
            }

            var v = NAPI.Vehicle.CreateVehicle(weaponHash, player.Position, player.Rotation, -1, -1, "TR3V1Z4");
            player.SetIntoVehicle(v, (int)VehicleSeat.Driver);
        }

        [Command("p")]
        public void CMD_p(Client sender)
        {
            NAPI.Util.ConsoleOutput($"{sender.Position.X.ToString().Replace(",", ".")}, {sender.Position.Y.ToString().Replace(",", ".")}, {sender.Position.Z.ToString().Replace(",", ".")}");
        }

        [Command("rot")]
        public void CMD_rot(Client sender)
        {
            NAPI.Util.ConsoleOutput($"{sender.Rotation.X.ToString().Replace(",", ".")}, {sender.Rotation.Y.ToString().Replace(",", ".")}, {sender.Rotation.Z.ToString().Replace(",", ".")}");
        }

        [Command("pos")]
        public void CMD_pos(Client sender, float x, float y, float z)
        {
            sender.Position = new Vector3(x, y, z);
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

        [Command("teta")]
        public void CMD_rottata(Client player)
        {
            NAPI.ClientEvent.TriggerClientEvent(player, "teta");
        }

        [RemoteEvent("teta2")]
        public void CMD_rottateta2ta(Client player, object a)
        {
            NAPI.Util.ConsoleOutput(JsonConvert.SerializeObject(a));
        }

        [Command("cenario")]
        public void CMD_cenario(Client player, int opc)
        {
            switch (opc)
            {
                case 1: player.PlayScenario("WORLD_HUMAN_AA_COFFEE"); break;
                case 2: player.PlayScenario("WORLD_HUMAN_AA_SMOKE"); break;
                case 3: player.PlayScenario("WORLD_HUMAN_BINOCULARS"); break;
                case 4: player.PlayScenario("WORLD_HUMAN_BUM_FREEWAY"); break;
                case 5: player.PlayScenario("WORLD_HUMAN_BUM_SLUMPED"); break;
                case 6: player.PlayScenario("WORLD_HUMAN_BUM_STANDING"); break;
                case 7: player.PlayScenario("WORLD_HUMAN_BUM_WASH"); break;
                case 8: player.PlayScenario("WORLD_HUMAN_CAR_PARK_ATTENDANT"); break;

                case 9: player.PlayScenario("WORLD_HUMAN_CHEERING"); break;
                case 10:
                    player.PlayScenario("WORLD_HUMAN_CLIPBOARD"); break;
                case 11:
                    player.PlayScenario("WORLD_HUMAN_CONST_DRILL"); break; //Emprego Manutenção
                case 12:
                    player.PlayScenario("WORLD_HUMAN_COP_IDLES"); break;
                case 13:
                    player.PlayScenario("WORLD_HUMAN_DRINKING"); break;
                case 14:
                    player.PlayScenario("WORLD_HUMAN_DRUG_DEALER"); break;
                case 15:
                    player.PlayScenario("WORLD_HUMAN_DRUG_DEALER_HARD"); break;
                case 16:
                    player.PlayScenario("WORLD_HUMAN_MOBILE_FILM_SHOCKING"); break;
                case 17:
                    player.PlayScenario("WORLD_HUMAN_GARDENER_LEAF_BLOWER"); break; //Soprando plantas
                case 18:
                    player.PlayScenario("WORLD_HUMAN_GARDENER_PLANT"); break; //Jardineiro no chão
                case 19:
                    player.PlayScenario("WORLD_HUMAN_GOLF_PLAYER"); break;
                case 20:
                    player.PlayScenario("WORLD_HUMAN_GUARD_PATROL"); break;
                case 21:
                    player.PlayScenario("WORLD_HUMAN_GUARD_STAND"); break;
                case 22:
                    player.PlayScenario("WORLD_HUMAN_GUARD_STAND_ARMY"); break;
                case 23:
                    player.PlayScenario("WORLD_HUMAN_HAMMERING"); break; //Martelando - Emprego Manutenção
                case 24:
                    player.PlayScenario("WORLD_HUMAN_HANG_OUT_STREET"); break;
                case 25:
                    player.PlayScenario("WORLD_HUMAN_HIKER_STANDING"); break;
                case 26:
                    player.PlayScenario("WORLD_HUMAN_HUMAN_STATUE"); break;
                case 27:
                    player.PlayScenario("WORLD_HUMAN_JANITOR"); break;
                case 28:
                    player.PlayScenario("WORLD_HUMAN_JOG_STANDING"); break;
                case 29:
                    player.PlayScenario("WORLD_HUMAN_LEANING"); break;
                case 30:
                    player.PlayScenario("WORLD_HUMAN_MAID_CLEAN"); break;
                case 31:
                    player.PlayScenario("WORLD_HUMAN_MUSCLE_FLEX"); break;
                case 32:
                    player.PlayScenario("WORLD_HUMAN_MUSCLE_FREE_WEIGHTS"); break; //Levantando peso
                case 33:
                    player.PlayScenario("WORLD_HUMAN_MUSICIAN"); break; //Tocando violão
                case 34:
                    player.PlayScenario("WORLD_HUMAN_PAPARAZZI"); break; //Camera Fotografica
                case 35:
                    player.PlayScenario("WORLD_HUMAN_PARTYING"); break;
                case 36:
                    player.PlayScenario("WORLD_HUMAN_PICNIC"); break; //Sentado no chão
                case 37:
                    player.PlayScenario("WORLD_HUMAN_PROSTITUTE_HIGH_CLASS"); break;
                case 38:
                    player.PlayScenario("WORLD_HUMAN_PROSTITUTE_LOW_CLASS"); break;
                case 39:
                    player.PlayScenario("WORLD_HUMAN_PUSH_UPS"); break; //Flexão
                case 40:
                    player.PlayScenario("WORLD_HUMAN_SEAT_LEDGE"); break;
                case 41:
                    player.PlayScenario("WORLD_HUMAN_SEAT_LEDGE_EATING"); break;
                case 42:
                    player.PlayScenario("WORLD_HUMAN_SEAT_STEPS"); break;
                case 43:
                    player.PlayScenario("WORLD_HUMAN_SEAT_WALL"); break;
                case 44:
                    player.PlayScenario("WORLD_HUMAN_SEAT_WALL_EATING"); break;
                case 45:
                    player.PlayScenario("WORLD_HUMAN_SEAT_WALL_TABLET"); break;
                case 46:
                    player.PlayScenario("WORLD_HUMAN_SECURITY_SHINE_TORCH"); break;
                case 47:
                    player.PlayScenario("WORLD_HUMAN_SIT_UPS"); break; //Abdominal
                case 48:
                    player.PlayScenario("WORLD_HUMAN_SMOKING"); break; //Fumando parado
                case 49:
                    player.PlayScenario("WORLD_HUMAN_SMOKING_POT"); break;
                case 50:
                    player.PlayScenario("WORLD_HUMAN_STAND_FIRE"); break;
                case 51:
                    player.PlayScenario("WORLD_HUMAN_STAND_FISHING"); break; //Pescando - Com vara
                case 52:
                    player.PlayScenario("WORLD_HUMAN_STAND_IMPATIENT"); break;
                case 53:
                    player.PlayScenario("WORLD_HUMAN_STAND_IMPATIENT_UPRIGHT"); break;
                case 54:
                    player.PlayScenario("WORLD_HUMAN_STAND_MOBILE"); break; //Parado mexendo no celular
                case 55:
                    player.PlayScenario("WORLD_HUMAN_STAND_MOBILE_UPRIGHT"); break;
                case 56:
                    player.PlayScenario("WORLD_HUMAN_STRIP_WATCH_STAND"); break; // Dançando suave
                case 57:
                    player.PlayScenario("WORLD_HUMAN_STUPOR"); break; //sentado e encostado
                case 58:
                    player.PlayScenario("WORLD_HUMAN_SUNBATHE"); break;//Deitado de bruço
                case 59:
                    player.PlayScenario("WORLD_HUMAN_SUNBATHE_BACK"); break;//Deitado suave
                case 60:
                    player.PlayScenario("WORLD_HUMAN_SUPERHERO"); break;
                case 61:
                    player.PlayScenario("WORLD_HUMAN_SWIMMING"); break;
                case 62:
                    player.PlayScenario("WORLD_HUMAN_TENNIS_PLAYER"); break; //Segurando raquete de tennis
                case 63:
                    player.PlayScenario("WORLD_HUMAN_TOURIST_MAP"); break; //Segurando mapa
                case 64:
                    player.PlayScenario("WORLD_HUMAN_TOURIST_MOBILE"); break; //
                case 65:
                    player.PlayScenario("WORLD_HUMAN_VEHICLE_MECHANIC"); break; // Em baixo do veiculo mexendo
                case 66:
                    player.PlayScenario("WORLD_HUMAN_WELDING"); break; //Usando Maçarico - Emprego Manutenção
                case 67:
                    player.PlayScenario("WORLD_HUMAN_WINDOW_SHOP_BROWSE"); break;
                case 68:
                    player.PlayScenario("WORLD_HUMAN_YOGA"); break; //Fazendo Yoga
                case 69:
                    player.PlayScenario("WORLD_BOAR_GRAZING"); break;
                case 70:
                    player.PlayScenario("WORLD_CAT_SLEEPING_GROUND"); break;
                case 71:
                    player.PlayScenario("WORLD_CAT_SLEEPING_LEDGE"); break;
                case 72:
                    player.PlayScenario("WORLD_COW_GRAZING"); break;
                case 73:
                    player.PlayScenario("WORLD_COYOTE_HOWL"); break;
                case 74:
                    player.PlayScenario("WORLD_COYOTE_REST"); break;
                case 75:
                    player.PlayScenario("WORLD_COYOTE_WANDER"); break;
                case 76:
                    player.PlayScenario("WORLD_CHICKENHAWK_FEEDING"); break;
                case 77:
                    player.PlayScenario("WORLD_CHICKENHAWK_STANDING"); break;
                case 78:
                    player.PlayScenario("WORLD_CORMORANT_STANDING"); break;
                case 79:
                    player.PlayScenario("WORLD_CROW_FEEDING"); break;
                case 80:
                    player.PlayScenario("WORLD_CROW_STANDING"); break;
                case 81:
                    player.PlayScenario("WORLD_DEER_GRAZING"); break;
                case 82:
                    player.PlayScenario("WORLD_DOG_BARKING_ROTTWEILER"); break;
                case 83:
                    player.PlayScenario("WORLD_DOG_BARKING_RETRIEVER"); break;
                case 84:
                    player.PlayScenario("WORLD_DOG_BARKING_SHEPHERD"); break;
                case 85:
                    player.PlayScenario("WORLD_DOG_SITTING_ROTTWEILER"); break;
                case 86:
                    player.PlayScenario("WORLD_DOG_SITTING_RETRIEVER"); break;
                case 87:
                    player.PlayScenario("WORLD_DOG_SITTING_SHEPHERD"); break;
                case 88:
                    player.PlayScenario("WORLD_DOG_BARKING_SMALL"); break;
                case 89:
                    player.PlayScenario("WORLD_DOG_SITTING_SMALL"); break;
                case 90:
                    player.PlayScenario("WORLD_FISH_IDLE"); break;
                case 100:
                    player.PlayScenario("WORLD_GULL_FEEDING"); break;
                case 101:
                    player.PlayScenario("WORLD_GULL_STANDING"); break;
                case 102:
                    player.PlayScenario("WORLD_HEN_PECKING"); break;
                case 103:
                    player.PlayScenario("WORLD_HEN_STANDING"); break;
                case 104:
                    player.PlayScenario("WORLD_MOUNTAIN_LION_REST"); break;
                case 105:
                    player.PlayScenario("WORLD_MOUNTAIN_LION_WANDER"); break;
                case 106:
                    player.PlayScenario("WORLD_PIG_GRAZING"); break;
                case 107:
                    player.PlayScenario("WORLD_PIGEON_FEEDING"); break;
                case 108:
                    player.PlayScenario("WORLD_PIGEON_STANDING"); break;
                case 109:
                    player.PlayScenario("WORLD_RABBIT_EATING"); break;
                case 110:
                    player.PlayScenario("WORLD_RATS_EATING"); break;
                case 111:
                    player.PlayScenario("WORLD_SHARK_SWIM"); break;
                case 112:
                    player.PlayScenario("PROP_BIRD_IN_TREE"); break;
                case 113:
                    player.PlayScenario("PROP_BIRD_TELEGRAPH_POLE"); break;
                case 114:
                    player.PlayScenario("PROP_HUMAN_ATM"); break; // Usando ATM
                case 115:
                    player.PlayScenario("PROP_HUMAN_BBQ"); break; // Fazendo Churrasco
                case 116:
                    player.PlayScenario("PROP_HUMAN_BUM_BIN"); break;
                case 117:
                    player.PlayScenario("PROP_HUMAN_BUM_SHOPPING_CART"); break; // Apioado no balcão
                case 118:
                    player.PlayScenario("PROP_HUMAN_MUSCLE_CHIN_UPS"); break; //Fazendo barras
                case 119:
                    player.PlayScenario("PROP_HUMAN_MUSCLE_CHIN_UPS_ARMY"); break; //Fazendo barras 2
                case 120:
                    player.PlayScenario("PROP_HUMAN_MUSCLE_CHIN_UPS_PRISON"); break; //Fazendo Barras 3
                case 121:
                    player.PlayScenario("PROP_HUMAN_PARKING_METER"); break;
                case 122:
                    player.PlayScenario("PROP_HUMAN_SEAT_ARMCHAIR"); break;
                case 123:
                    player.PlayScenario("PROP_HUMAN_SEAT_BAR"); break;
                case 124:
                    player.PlayScenario("PROP_HUMAN_SEAT_BENCH"); break;
                case 125:
                    player.PlayScenario("PROP_HUMAN_SEAT_BENCH_DRINK"); break;
                case 126:
                    player.PlayScenario("PROP_HUMAN_SEAT_BENCH_DRINK_BEER"); break;
                case 127:
                    player.PlayScenario("PROP_HUMAN_SEAT_BENCH_FOOD"); break;
                case 128:
                    player.PlayScenario("PROP_HUMAN_SEAT_BUS_STOP_WAIT"); break;
                case 129:
                    player.PlayScenario("PROP_HUMAN_SEAT_CHAIR"); break;
                case 130:
                    player.PlayScenario("PROP_HUMAN_SEAT_CHAIR_DRINK"); break;
                case 131:
                    player.PlayScenario("PROP_HUMAN_SEAT_CHAIR_DRINK_BEER"); break;
                case 132:
                    player.PlayScenario("PROP_HUMAN_SEAT_CHAIR_FOOD"); break;
                case 134:
                    player.PlayScenario("PROP_HUMAN_SEAT_CHAIR_UPRIGHT"); break;
                case 135:
                    player.PlayScenario("PROP_HUMAN_SEAT_CHAIR_MP_PLAYER"); break;
                case 136:
                    player.PlayScenario("PROP_HUMAN_SEAT_COMPUTER"); break;
                case 137:
                    player.PlayScenario("PROP_HUMAN_SEAT_DECKCHAIR"); break;
                case 138:
                    player.PlayScenario("PROP_HUMAN_SEAT_DECKCHAIR_DRINK"); break;
                case 139:
                    player.PlayScenario("PROP_HUMAN_SEAT_MUSCLE_BENCH_PRESS"); break;
                case 140:
                    player.PlayScenario("PROP_HUMAN_SEAT_MUSCLE_BENCH_PRESS_PRISON"); break;
                case 141:
                    player.PlayScenario("PROP_HUMAN_SEAT_SEWING"); break;
                case 142:
                    player.PlayScenario("PROP_HUMAN_SEAT_STRIP_WATCH"); break;
                case 143:
                    player.PlayScenario("PROP_HUMAN_SEAT_SUNLOUNGER"); break;
                case 144:
                    player.PlayScenario("PROP_HUMAN_STAND_IMPATIENT"); break;
                case 145:
                    player.PlayScenario("CODE_HUMAN_COWER"); break;
                case 146:
                    player.PlayScenario("CODE_HUMAN_CROSS_ROAD_WAIT"); break;
                case 147:
                    player.PlayScenario("CODE_HUMAN_PARK_CAR"); break;
                case 148:
                    player.PlayScenario("PROP_HUMAN_MOVIE_BULB"); break;
                case 149:
                    player.PlayScenario("PROP_HUMAN_MOVIE_STUDIO_LIGHT"); break;
                case 150:
                    player.PlayScenario("CODE_HUMAN_MEDIC_KNEEL"); break; //agaixado olhando
                case 151:
                    player.PlayScenario("CODE_HUMAN_MEDIC_TEND_TO_DEAD"); break;
                case 152:
                    player.PlayScenario("CODE_HUMAN_MEDIC_TIME_OF_DEATH"); break; //Anotando algo no caderno
                case 153:
                    player.PlayScenario("CODE_HUMAN_POLICE_CROWD_CONTROL"); break;
                case 154:
                    player.PlayScenario("CODE_HUMAN_POLICE_INVESTIGATE"); break;
                case 155:
                    player.PlayScenario("CODE_HUMAN_STAND_COWER"); break;
                case 156:
                    player.PlayScenario("EAR_TO_TEXT"); break;
                case 157:
                    player.PlayScenario("EAR_TO_TEXT_FAT"); break;
            }
        }

        [Command("i")]
        public void CMD_i(Client player, string ipl) => NAPI.ClientEvent.TriggerClientEvent(player, "setIPL", ipl);

        [Command("ri")]
        public void CMD_ri(Client player, string ipl) => NAPI.ClientEvent.TriggerClientEvent(player, "removeIPL", ipl);

        [Command("vh")]
        public void CMD_vh(Client player)
        {
            if (!player.IsInVehicle)
                return;

            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"Motor: {player.Vehicle.EngineStatus}");
            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"Motor Health: {NAPI.Vehicle.GetVehicleEngineHealth(player.Vehicle)}");
            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"Body Health: {NAPI.Vehicle.GetVehicleBodyHealth(player.Vehicle)}");
        }
    }
}