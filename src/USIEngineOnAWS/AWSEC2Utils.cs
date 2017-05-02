using Amazon.EC2;
using Amazon.EC2.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace USIEngineOnAWS
{
    delegate void write_log_method(string text);

    class AWSEC2Utils : IDisposable
    {
        public string region { get; set; }
        public string vpc_id { get; set; }
        public string internet_gateway_id { get; set; }
        public string routetable_id { get; set; }
        public Dictionary<string, string> subnet_ids =
            new Dictionary<string, string>() { { "a", "" }, { "b", "" }, { "c", "" }, { "d", "" }, { "e", "" }, { "f", "" }, { "g", "" } };
        public string security_group_id { get; set; }
        public string image_id { get; set; }
        public string spot_request_id { get; set; }

        static public string[] paravirtual_instance_types = new string[] {
                "m1.small", "m1.medium", "m1.large", "m1.xlarge",
                "c1.medium", "c1.xlarge", "cc2.8xlarge", "cg1.4xlarge",
                "m2.xlarge", "m2.2xlarge", "m2.4xlarge", "cr1.8xlarge",
                "hi1.4xlarge", "hs1.8xlarge", "t1.micro"};

        private InifileUtils setting_;
        private write_log_method write_log;
        private AmazonEC2Client instance = null;

        public AWSEC2Utils(InifileUtils setting, write_log_method func)
        {
            setting_ = setting;
            write_log = func;
        }

        private AmazonEC2Client get_client()
        {
            if (instance != null) return instance;

            AmazonEC2Config config = new AmazonEC2Config();
            config.ServiceURL = "https://ec2." + region + ".amazonaws.com";
            instance = new AmazonEC2Client(
                Globals.aws_access_key_id.Trim(), Globals.aws_secret_access_key.Trim(), config);
            return instance;
        }

        public void set_name_tag(AmazonEC2Client client, string res_id, string val)
        {
            var req = new CreateTagsRequest();
            req.Resources.Add(res_id);
            req.Tags.Add(new Tag("Name", val));
            client.CreateTags(req);
        }
        public void set_name_tag(string id, string tag)
        {
            var client = get_client();
            set_name_tag(client, id, tag);
        }

        public bool load_vpc_id()
        {
            write_log(region + " の VPC を確認しています。");
            try
            {
                var client = get_client();
                var query_req = new DescribeVpcsRequest();
                query_req.Filters.Add(new Filter() { Name = "tag-value", Values = new List<string>() { Helper.build_name(setting_, "vpc") } });
                var query_res = client.DescribeVpcs(query_req);
                if (query_res.Vpcs.Count != 0)
                {
                    vpc_id = query_res.Vpcs[0].VpcId;
                    return true;
                }

                write_log(region + " に VPC を作成しています。");
                var create_req = new CreateVpcRequest();
                create_req.CidrBlock = "10.0.0.0/16";
                var create_res = client.CreateVpc(create_req);
                set_name_tag(client, create_res.Vpc.VpcId, Helper.build_name(setting_, "vpc"));
                vpc_id = create_res.Vpc.VpcId;
            }
            catch (Exception ex)
            {
                write_log("ERROR: " + ex.ToString());
                return false;
            }
            return true;
        }

        public bool load_internet_gateway_id()
        {
            write_log(region + " のインターネットゲートウェイを確認しています。");
            try
            {
                var client = get_client();
                var query_req = new DescribeInternetGatewaysRequest();
                query_req.Filters.Add(new Filter() { Name = "tag-value", Values = new List<string>() { Helper.build_name(setting_, "igw") } });
                var query_res = client.DescribeInternetGateways(query_req);
                if (query_res.InternetGateways.Count != 0)
                {
                    foreach (var row in query_res.InternetGateways[0].Attachments)
                    {
                        if (row.VpcId.Equals(vpc_id))
                        {
                            internet_gateway_id = query_res.InternetGateways[0].InternetGatewayId;
                            write_log(vpc_id + " のインターネットゲートウェイは " + internet_gateway_id + " です");
                            return true;
                        }
                    }
                }

                write_log(region + " にインターネットゲートウェイを作成しています。");
                var create_req = new CreateInternetGatewayRequest();
                var create_res = client.CreateInternetGateway(create_req);

                set_name_tag(client, create_res.InternetGateway.InternetGatewayId, Helper.build_name(setting_, "igw"));
                internet_gateway_id = create_res.InternetGateway.InternetGatewayId;
                write_log("インターネットゲートウェイ " + internet_gateway_id + " を作成しました。");


                write_log("VPC " + vpc_id + " に " + internet_gateway_id + " を関連付けます。");
                var attach_req = new AttachInternetGatewayRequest();
                attach_req.InternetGatewayId = internet_gateway_id;
                attach_req.VpcId = vpc_id;
                client.AttachInternetGateway(attach_req);

            }
            catch (Exception ex)
            {
                write_log("ERROR: " + ex.ToString());
                return false;
            }
            return true;
        }

        public bool load_routetable_id()
        {
            write_log(vpc_id + " のルートデーブルを確認しています。");
            try
            {
                var client = get_client();
                var query_req = new DescribeRouteTablesRequest();

                query_req.Filters.Add(new Filter() { Name = "vpc-id", Values = new List<string>() { vpc_id } });
                var query_res = client.DescribeRouteTables(query_req);
                routetable_id = query_res.RouteTables[0].RouteTableId;
                write_log(vpc_id + " のルートデーブルは " + routetable_id + " です");

                foreach (var row in query_res.RouteTables[0].Routes)
                {
                    if (row.GatewayId.Equals(internet_gateway_id) && row.State == RouteState.Active)
                        return true;
                }
                set_name_tag(client, query_res.RouteTables[0].RouteTableId, Helper.build_name(setting_, "rtb"));
                write_log("インターネットゲートウェイ " + internet_gateway_id + " に " + routetable_id + " を関連付けます。");
                var update_req = new CreateRouteRequest();
                update_req.RouteTableId = routetable_id;
                update_req.DestinationCidrBlock = "0.0.0.0/0";
                update_req.GatewayId = internet_gateway_id;
                client.CreateRoute(update_req);

            }
            catch (Exception ex)
            {
                write_log("ERROR: " + ex.ToString());
                return false;
            }
            return true;
        }

        public bool load_subnet_id(string tag)
        {
            write_log(region + tag + " のサブネットを確認しています。");

            try
            {
                var client = get_client();
                var query_req = new DescribeSubnetsRequest();
                query_req.Filters.Add(new Filter() { Name = "tag-value", Values = new List<string>() { Helper.build_name(setting_, "subnet_" + tag) } });
                var query_res = client.DescribeSubnets(query_req);
                if (query_res.Subnets.Count != 0 && query_res.Subnets[0].VpcId.Equals(vpc_id))
                {
                    subnet_ids[tag] = query_res.Subnets[0].SubnetId;
                    write_log(region + tag + " のサブネットは " + subnet_ids[tag] + " です");
                    return true;

                }

                var create_req = new CreateSubnetRequest();
                if (tag.Equals("a"))
                    create_req.CidrBlock = "10.0.0.0/24";
                else if (tag.Equals("b"))
                    create_req.CidrBlock = "10.0.1.0/24";
                else if (tag.Equals("c"))
                    create_req.CidrBlock = "10.0.2.0/24";
                else if (tag.Equals("d"))
                    create_req.CidrBlock = "10.0.3.0/24";
                else if (tag.Equals("e"))
                    create_req.CidrBlock = "10.0.4.0/24";
                else if (tag.Equals("f"))
                    create_req.CidrBlock = "10.0.5.0/24";
                else if (tag.Equals("g"))
                    create_req.CidrBlock = "10.0.6.0/24";
                create_req.VpcId = vpc_id;
                create_req.AvailabilityZone = region + tag;

                var create_res = client.CreateSubnet(create_req);
                subnet_ids[tag] = create_res.Subnet.SubnetId;
                write_log("サブネット " + subnet_ids[tag] + " を作成しました。");

                set_name_tag(client, subnet_ids[tag], Helper.build_name(setting_, "subnet_" + tag));
            }
            catch (Exception ex)
            {
                write_log("ERROR: " + ex.ToString());
                return false;
            }

            return true;
        }

        public bool load_security_group_id()
        {
            write_log(region + " のセキュリティグループを確認しています。");
            try
            {
                var client = get_client();
                var query_req = new DescribeSecurityGroupsRequest();
                query_req.Filters.Add(new Filter() { Name = "tag-value", Values = new List<string>() { Helper.build_name(setting_, "sg") } });
                var query_res = client.DescribeSecurityGroups(query_req);
                write_log("自 IPAddress を確認しています。");
                string ipaddress = Helper.get_remote_ipaddress() + "/32";
                if (query_res.SecurityGroups.Count != 0)
                {
                    security_group_id = query_res.SecurityGroups[0].GroupId;

                    foreach (var row in query_res.SecurityGroups[0].IpPermissions)
                    {
                        foreach (var row2 in row.IpRanges)
                        {
                            if (row2.Equals(ipaddress))
                            {
                                write_log(region + " のセキュリティグループは " + security_group_id + " です");
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    write_log(region + " にセキュリティグループを作成しています。");
                    var create_req = new CreateSecurityGroupRequest();
                    create_req.VpcId = vpc_id;
                    create_req.GroupName = "ingress_ssh-sg";
                    create_req.Description = "from specific ipaddress.";
                    var create_res = client.CreateSecurityGroup(create_req);
                    security_group_id = create_res.GroupId;
                    set_name_tag(client, security_group_id, Helper.build_name(setting_, "sg"));
                    write_log("セキュリティグループ " + security_group_id + " を作成しました。");

                }

                write_log("セキュリティグループ " + security_group_id + " に " + ipaddress + " を関連付けます。");
                IpPermission ipPermission_22 = new IpPermission();
                ipPermission_22.ToPort = 22;
                ipPermission_22.FromPort = 22;
                ipPermission_22.IpProtocol = "6";
                ipPermission_22.IpRanges.Add(ipaddress);

                IpPermission ipPermission_53556 = new IpPermission();
                ipPermission_53556.ToPort = 53556;
                ipPermission_53556.FromPort = 53556;
                ipPermission_53556.IpProtocol = "6";
                ipPermission_53556.IpRanges.Add(ipaddress);

                var authorize_req = new AuthorizeSecurityGroupIngressRequest();
                authorize_req.GroupId = security_group_id;
                authorize_req.IpPermissions = new List<IpPermission>() { ipPermission_22, ipPermission_53556 };

                client.AuthorizeSecurityGroupIngress(authorize_req);
                set_name_tag(client, security_group_id, Helper.build_name(setting_, "sg"));
                write_log("セキュリティグループ " + security_group_id + " を作成しました。");
            }
            catch (Exception ex)
            {
                write_log("ERROR: " + ex.ToString());
                return false;
            }
            return true;
        }

        public bool delete_vpc_all(string region_txt)
        {
            try
            {
                var client = get_client();
                write_log(region + " の VPC を確認しています。");
                var query_req = new DescribeVpcsRequest();
                query_req.Filters.Add(new Filter() { Name = "tag-value", Values = new List<string>() { Helper.build_name(setting_, "vpc") } });
                var query_res = client.DescribeVpcs(query_req);
                if (query_res.Vpcs.Count == 0)
                {
                    write_log(region + " の VPC が存在しませんでした。");
                    return true;
                }
                vpc_id = query_res.Vpcs[0].VpcId;

                var query_instance_req = new DescribeInstancesRequest();
                var query_instance_res = client.DescribeInstances(query_instance_req);

                bool flg = true;
                foreach (var row in query_instance_res.Reservations)
                {
                    foreach (var row2 in row.Instances)
                    {
                        if (row2.State != null && row2.State.Code != 48
                            && row2.VpcId != null && row2.VpcId.Equals(vpc_id))
                        {
                            flg = false;
                            break;
                        }
                    }
                    if (flg == false) break;
                }
                if (flg == false)
                {
                    write_log(region + " にはインスタンスが存在する為、VPC を削除できません。");
                    return false;
                }

                write_log(region + " のセキュリティグループを確認しています。");

                var query_sg_req = new DescribeSecurityGroupsRequest();
                query_sg_req.Filters.Add(new Filter() { Name = "tag-value", Values = new List<string>() { Helper.build_name(setting_, "sg") } });
                var query_sg_res = client.DescribeSecurityGroups(query_sg_req);
                foreach (var row in query_sg_res.SecurityGroups)
                {
                    write_log("セキュリティグループ " + row.GroupId + " を削除しています。");
                    var delete_sg_req = new DeleteSecurityGroupRequest();
                    delete_sg_req.GroupId = row.GroupId;
                    client.DeleteSecurityGroup(delete_sg_req);
                }

                using (FileStream fs = new FileStream(region_txt, FileMode.Open, FileAccess.Read))
                {
                    IList<string> list = new List<string>(subnet_ids.Keys);
                    foreach (string tag in list)
                    {
                        if (Helper.check_az(fs, region, tag, 0) == false) continue;
                        write_log(region + tag + " のサブネットを確認しています。");
                        var query_sub_req = new DescribeSubnetsRequest();
                        query_sub_req.Filters.Add(new Filter() { Name = "tag-value", Values = new List<string>() { Helper.build_name(setting_, "subnet_" + tag) } });
                        var query_sub_res = client.DescribeSubnets(query_sub_req);
                        foreach (var row in query_sub_res.Subnets)
                        {
                            write_log("サブネット " + row.SubnetId + " を削除しています。");
                            var delete_sub_req = new DeleteSubnetRequest();
                            delete_sub_req.SubnetId = row.SubnetId;
                            client.DeleteSubnet(delete_sub_req);
                        }
                    }
                }

                write_log(region + " のインターネットゲートウェイを確認しています。");

                var query_igw_req = new DescribeInternetGatewaysRequest();
                query_igw_req.Filters.Add(new Filter() { Name = "tag-value", Values = new List<string>() { Helper.build_name(setting_, "igw") } });
                var query_igw_res = client.DescribeInternetGateways(query_igw_req);
                foreach (var row in query_igw_res.InternetGateways)
                {
                    var detach_igw_req = new DetachInternetGatewayRequest();
                    detach_igw_req.InternetGatewayId = row.InternetGatewayId;
                    detach_igw_req.VpcId = vpc_id;
                    client.DetachInternetGateway(detach_igw_req);

                    var delete_igw_req = new DeleteInternetGatewayRequest();
                    delete_igw_req.InternetGatewayId = row.InternetGatewayId;
                    client.DeleteInternetGateway(delete_igw_req);
                }

                write_log("VPC " + vpc_id + " を削除しています。");

                var delete_vpc_req = new DeleteVpcRequest();
                delete_vpc_req.VpcId = vpc_id;
                client.DeleteVpc(delete_vpc_req);

            }
            catch (Exception ex)
            {
                write_log("ERROR: " + ex.ToString());
                return false;
            }
            return true;
        }

        public decimal load_spot_price(string instance_type, string az)
        {
            try
            {
                var client = get_client();
                var query_req = new DescribeSpotPriceHistoryRequest();
                query_req.InstanceTypes.Add(instance_type);
                query_req.AvailabilityZone = region + az;
                query_req.ProductDescriptions.Add("Linux/UNIX");
                query_req.StartTime = DateTime.Now;
                query_req.MaxResults = 1;
                var query_res = client.DescribeSpotPriceHistory(query_req);
                if (query_res.SpotPriceHistory.Count == 0)
                {
                    write_log(region + az + " にはスポットインスタンスがありません。");
                    return 10000;
                }
                write_log(region + az + " のスポットインスタンスの現在の値段 " + query_res.SpotPriceHistory[0].Price);
                return Convert.ToDecimal(query_res.SpotPriceHistory[0].Price);

            }
            catch (Exception ex)
            {
                write_log("ERROR: " + ex.ToString());
                return 10000;
            }
        }

        public bool reload_key_pair(string private_key, string tag)
        {
            write_log("鍵を生成します。");
            try
            {
                var client = get_client();

                write_log(region + " のキーペアを削除しています。");
                var delete_req = new DeleteKeyPairRequest();
                delete_req.KeyName = Helper.build_name(setting_, tag);
                client.DeleteKeyPair(delete_req);

                var create_req = new CreateKeyPairRequest();
                create_req.KeyName = Helper.build_name(setting_, tag);
                var create_res = client.CreateKeyPair(create_req);

                write_log("キーペア保存をしています。");
                File.WriteAllText(private_key, create_res.KeyPair.KeyMaterial);
            }
            catch (Exception ex)
            {
                write_log("ERROR: " + ex.ToString());
                return false;
            }
            return true;
        }

        public bool delete_key_pair(string tag)
        {
            try
            {
                var client = get_client();
                var delete_key_req = new DeleteKeyPairRequest();
                delete_key_req.KeyName = Helper.build_name(setting_, tag);
                client.DeleteKeyPair(delete_key_req);

            }
            catch (Exception ex)
            {
                write_log("ERROR: " + ex.ToString());
                return false;
            }
            return true;
        }

        public bool check_vpc_id()
        {
            write_log(region + " の VPC を確認しています。");
            try
            {
                var client = get_client();
                var query_req = new DescribeVpcsRequest();
                query_req.Filters.Add(new Filter() { Name = "tag-value", Values = new List<string>() { Helper.build_name(setting_, "vpc") } });
                var query_res = client.DescribeVpcs(query_req);
                if (query_res.Vpcs.Count == 0) return false;

                vpc_id = query_res.Vpcs[0].VpcId;
            }
            catch (Exception ex)
            {
                write_log("ERROR: " + ex.ToString());
                return false;
            }
            return true;
        }

        public bool check_subnet_id(string tag)
        {
            write_log(region + tag + " のサブネットを確認しています。");
            try
            {
                var client = get_client();
                var query_req = new DescribeSubnetsRequest();
                query_req.Filters.Add(new Filter() { Name = "tag-value", Values = new List<string>() { Helper.build_name(setting_, "subnet_" + tag) } });
                var query_res = client.DescribeSubnets(query_req);
                if (query_res.Subnets.Count == 0 || !query_res.Subnets[0].VpcId.Equals(vpc_id))
                    return false;
                subnet_ids[tag] = query_res.Subnets[0].SubnetId;
            }
            catch (Exception ex)
            {
                write_log("ERROR: " + ex.ToString());
                return false;
            }
            return true;
        }

        public bool check_security_group_id()
        {
            return load_security_group_id();
        }

        public bool load_image_id(string vm_type)
        {
            write_log(region + " のマシンイメージを確認しています。");
            try
            {
                string tmp = setting_.getValueString("common", "image_id");
                if (tmp != null && tmp.Length != 0)
                {
                    image_id = tmp;
                    write_log("マシンイメージ " + image_id + " を使用します。");
                    return true;
                }
            }
            catch
            {
                //none
            }
            try
            {
                var client = get_client();
                var query_req = new DescribeImagesRequest();
                query_req.Filters.Add(new Filter() { Name = "name", Values = new List<string>() { "*/images/*/ubuntu-*-16.04-*" } });
                query_req.Filters.Add(new Filter() { Name = "virtualization-type", Values = new List<string>() { vm_type } });
                if(vm_type == "hvm")
                {
                    query_req.Filters.Add(new Filter() { Name = "root-device-type", Values = new List<string>() { "ebs" } });
                }
                var query_res = client.DescribeImages(query_req);
                Amazon.EC2.Model.Image[] images = query_res.Images.ToArray();
                images = images.OrderByDescending(n => n.CreationDate).ToArray();
                image_id = images[0].ImageId;
            }
            catch (Exception ex)
            {
                write_log("ERROR: " + ex.ToString());
                return false;
            }
            return true;
        }

        public bool request_spot(string instance_type, string availability_zone, string spot_price, string key_tag)
        {
            write_log(region + " に対してスポットリクエストを作成しています。");
            int nn = setting_.getValueInt("common", "request_spot_width_of_minutes");
            try
            {
                InstanceNetworkInterfaceSpecification instanceNetworkInterfaceSpecification = new InstanceNetworkInterfaceSpecification();
                instanceNetworkInterfaceSpecification.DeviceIndex = 0;
                instanceNetworkInterfaceSpecification.SubnetId = subnet_ids[availability_zone];
                instanceNetworkInterfaceSpecification.Groups.Add(security_group_id);
                instanceNetworkInterfaceSpecification.AssociatePublicIpAddress = true;

                LaunchSpecification launchSpecification = new LaunchSpecification();
                launchSpecification.ImageId = image_id;
                launchSpecification.KeyName = Helper.build_name(setting_, key_tag);
                launchSpecification.InstanceType = InstanceType.FindValue(instance_type);
                launchSpecification.Placement = new SpotPlacement(region + availability_zone);
                launchSpecification.NetworkInterfaces.Add(instanceNetworkInterfaceSpecification);

                var client = get_client();
                var spot_req = new RequestSpotInstancesRequest();
                spot_req.SpotPrice = spot_price;
                spot_req.InstanceCount = 1;
                spot_req.Type = SpotInstanceType.OneTime;
                spot_req.ValidUntil = DateTime.Now.AddMinutes(nn);
                spot_req.LaunchSpecification = launchSpecification;
                var query_res = client.RequestSpotInstances(spot_req);

                spot_request_id = query_res.SpotInstanceRequests[0].SpotInstanceRequestId;
            }
            catch (Exception ex)
            {
                write_log("ERROR: " + ex.ToString());
                return false;
            }
            return true;
        }

        public string query_request_spot()
        {
            try
            {
                var client = get_client();
                var query_req = new DescribeSpotInstanceRequestsRequest();
                query_req.SpotInstanceRequestIds.Add(spot_request_id);
                var query_res = client.DescribeSpotInstanceRequests(query_req);

                string status = query_res.SpotInstanceRequests[0].Status.Code;
                write_log("スポットリクエストの状態は " + status + " です。");
                string tmp = query_res.SpotInstanceRequests[0].InstanceId;
                if (tmp != null && tmp.Length != 0) status = status + "," + tmp;

                return status;
            }
            catch (Exception ex)
            {
                write_log("ERROR: " + ex.ToString());
                return null;
            }
        }

        public bool cancel_spot()
        {
            write_log("スポットリクエスト " + spot_request_id + " をキャンセルしています。");

            try
            {
                var client = get_client();
                var cancel_req = new CancelSpotInstanceRequestsRequest();
                cancel_req.SpotInstanceRequestIds.Add(spot_request_id);
                client.CancelSpotInstanceRequests(cancel_req);
            }
            catch (Exception ex)
            {
                write_log("ERROR: " + ex.ToString());
                return false;
            }

            return true;
        }

        public string get_instance_public_ipaddress(string instance_id)
        {
            write_log("インスタンス " + instance_id + " の情報を取得しています。");
            try
            {
                var client = get_client();
                var query_req = new DescribeInstancesRequest();
                query_req.InstanceIds.Add(instance_id);
                var query_res = client.DescribeInstances(query_req);
                return query_res.Reservations[0].Instances[0].PublicIpAddress;
            }
            catch (Exception ex)
            {
                write_log("ERROR: " + ex.ToString());
                return "";
            }
        }

        public void delete_instance(string instance_name)
        {
            write_log(region + " のインスタンス " + instance_name + " を確認しています。");
            try
            {
                var client = get_client();
                if (check_vpc_id() == false)
                {
                    write_log(region + " の VPC が存在しません。");
                    return;
                }
                var query_req = new DescribeInstancesRequest();
                var query_res = client.DescribeInstances(query_req);

                var delete_req = new TerminateInstancesRequest();
                foreach (var row in query_res.Reservations)
                {
                    foreach (var row2 in row.Instances)
                    {
                        bool flg = false;
                        foreach (var row3 in row2.Tags)
                        {
                            if (row3.Key.Equals("Name") && row3.Value.Equals(instance_name))
                            {
                                flg = true;
                            }
                        }
                        if (flg == false) continue;
                        if (row2.VpcId != null && row2.VpcId.Equals(vpc_id)
                            && row2.State != null && row2.State.Code <= 16)
                        {
                            write_log("インスタンス " + row2.InstanceId + " を削除します。");
                            set_name_tag(client, row2.InstanceId, "(terminate)");
                            delete_req.InstanceIds.Add(row2.InstanceId);
                        }
                    }
                }
                if (delete_req.InstanceIds.Count != 0)
                    client.TerminateInstances(delete_req);
            }
            catch (Exception ex)
            {
                write_log("ERROR: " + ex.ToString());
            }
        }

        public void delete_region_instance()
        {
            write_log(region + " のインスタンスを確認しています。");
            try
            {
                var client = get_client();
                if (check_vpc_id() == false) return;

                var query_req = new DescribeInstancesRequest();
                var query_res = client.DescribeInstances(query_req);

                var delete_req = new TerminateInstancesRequest();
                foreach (var row in query_res.Reservations)
                {
                    foreach (var row2 in row.Instances)
                    {
                        if (row2.VpcId == null || !row2.VpcId.Equals(vpc_id)) continue;
                        if (row2.State != null && row2.State.Code <= 16)
                        {
                            write_log("インスタンス " + row2.InstanceId + " を削除します。");
                            set_name_tag(client, row2.InstanceId, "(terminate)");
                            delete_req.InstanceIds.Add(row2.InstanceId);
                        }
                    }
                }
                if (delete_req.InstanceIds.Count != 0)
                    client.TerminateInstances(delete_req);
            }
            catch (Exception ex)
            {
                write_log("ERROR: " + ex.ToString());
            }
        }

        public void Dispose()
        {
            if (instance != null)
            {
                instance.Dispose();
            }
        }
    }
}
